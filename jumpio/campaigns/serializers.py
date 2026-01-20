from rest_framework import serializers
from .models import Campaign, CampaignLevel

class CampaignLevelSerializer(serializers.ModelSerializer):
    level_id = serializers.IntegerField(source="level.id", read_only=True)
    title = serializers.CharField(source="level.title", read_only=True)
    difficulty = serializers.CharField(source="level.difficulty", read_only=True)
    timer = serializers.IntegerField(source="level.timer", read_only=True)
    layout_file = serializers.CharField(source="level.layout_file.url", read_only=True)

    class Meta:
        model = CampaignLevel
        fields = ["order_index","level_id","title","difficulty","timer","layout_file"]



class CampaignSerializer(serializers.ModelSerializer):
    user = serializers.PrimaryKeyRelatedField(read_only=True)
    levels = CampaignLevelSerializer(source="campaign_levels", many=True, read_only=True)

    class Meta:
        model = Campaign
        fields = ["id", "title", "user", "description", "date_created", "levels"]
        read_only_fields = ["id", "user", "date_created", "levels"]


class CampaignLevelItemSerializer(serializers.Serializer):
    level_id = serializers.IntegerField()
    order_index = serializers.IntegerField(min_value=0)