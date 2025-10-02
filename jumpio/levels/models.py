from django.db import models
from django.conf import settings

class Level(models.Model):
    title = models.CharField(max_length=255)
    user = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    layout = models.JSONField()
    date_created = models.DateTimeField(auto_now_add=True)
    timer = models.IntegerField(null=True, blank=True) # in seconds
    difficulty = models.CharField(max_length=10)
    preview_image = models.ImageField(upload_to='levels/preview_imgs', null=True, blank=True)

class LevelAttempt(models.Model):
    level_id = models.ForeignKey(Level, on_delete=models.CASCADE)
    user_id = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    success = models.BooleanField
    timestamp = models.DateTimeField(auto_now_add=True)

