from rest_framework import serializers
from .models import Level

class LevelSerializer(serializers.ModelSerializer):

    class Meta:
        model = Level
        fields = ["id", "title", "user", "layout_file", "date_created", "timer", "difficulty", "preview_image"]
        read_only_fields = ["id", "user", "date_created"]