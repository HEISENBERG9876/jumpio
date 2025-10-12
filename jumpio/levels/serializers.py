from rest_framework import serializers
from .models import Level

class PlacedObjectSerializer(serializers.Serializer):
    id = serializers.CharField()
    x = serializers.IntegerField()
    y = serializers.IntegerField()
    width = serializers.IntegerField(required=False, default=1)
    height = serializers.IntegerField(required=False, default=1)
    rotation = serializers.IntegerField(required=False, default=0)
    metadata = serializers.JSONField(required=False)

class LevelSerializer(serializers.ModelSerializer):
    layout = PlacedObjectSerializer(many=True)

    class Meta:
        model = Level
        fields = ["id", "title", "user", "layout", "date_created", "timer", "difficulty", "preview_image"]
        read_only_fields = ["id", "user", "date_created"]