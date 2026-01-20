from django.db import transaction
from rest_framework import viewsets, permissions, status
from rest_framework.decorators import action
from rest_framework.response import Response

from .models import Campaign, CampaignLevel
from .serializers import CampaignSerializer, CampaignLevelItemSerializer
from levels.models import Level


class IsOwnerOrReadOnly(permissions.BasePermission):
    def has_object_permission(self, request, view, obj):
        if request.method in permissions.SAFE_METHODS:
            return True
        return obj.user == request.user


class CampaignViewSet(viewsets.ModelViewSet):
    queryset = Campaign.objects.select_related("user").order_by("-date_created")
    serializer_class = CampaignSerializer

    def get_permissions(self):
        return [permissions.IsAuthenticated(), IsOwnerOrReadOnly()]

    def perform_create(self, serializer):
        serializer.save(user=self.request.user)

    @action(detail=True, methods=["put"], url_path="levels")
    def set_levels(self, request, pk=None):
        campaign = self.get_object()

        levels_payload = request.data.get("levels")

        if levels_payload is None or not isinstance(levels_payload, list) or len(levels_payload) == 0:
            return Response(
                {"detail": "levels must be a non-empty list"},
                status=status.HTTP_400_BAD_REQUEST
            )

        serializer = CampaignLevelItemSerializer(data=levels_payload, many=True)
        serializer.is_valid(raise_exception=True)
        items = serializer.validated_data

        used_order_indexes = []
        used_level_ids = []

        for item in items:
            if item["order_index"] in used_order_indexes:
                return Response(
                    {"detail": "Duplicate order_index in payload"},
                    status=status.HTTP_400_BAD_REQUEST
                )
            used_order_indexes.append(item["order_index"])

            if item["level_id"] in used_level_ids:
                return Response(
                    {"detail": "Duplicate level_id in payload"},
                    status=status.HTTP_400_BAD_REQUEST
                )
            used_level_ids.append(item["level_id"])

        for level_id in used_level_ids:
            if not Level.objects.filter(id=level_id).exists():
                return Response(
                    {"detail": f"Level with id={level_id} does not exist"},
                    status=status.HTTP_400_BAD_REQUEST
                )

        with transaction.atomic():
            CampaignLevel.objects.filter(campaign=campaign).delete()

            for item in items:
                CampaignLevel.objects.create(
                    campaign=campaign,
                    level_id=item["level_id"],
                    order_index=item["order_index"]
                )

        campaign.refresh_from_db()
        return Response(
            CampaignSerializer(campaign, context={"request": request}).data,
            status=status.HTTP_200_OK
        )
