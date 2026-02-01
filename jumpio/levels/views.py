from rest_framework import viewsets, permissions, parsers

from .models import Level
from .serializers import LevelSerializer


class IsOwnerOrReadOnly(permissions.BasePermission):
    def has_object_permission(self, request, view, obj):
        if request.method in permissions.SAFE_METHODS:
            return True
        return obj.user == request.user


class LevelViewSet(viewsets.ModelViewSet):
    queryset = Level.objects.select_related("user").order_by("-date_created")
    serializer_class = LevelSerializer
    parser_classes = [parsers.MultiPartParser, parsers.FormParser]

    filterset_fields = ["user", "difficulty"]

    def perform_create(self, serializer):
        serializer.save(user=self.request.user)

    def get_permissions(self):
        return [permissions.IsAuthenticated(), IsOwnerOrReadOnly()]

