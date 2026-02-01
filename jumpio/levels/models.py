from django.db import models
from django.conf import settings
import uuid
import os


def unique_file_path(instance, filename, base_folder):
    ext = filename.split('.')[-1]
    unique_name = f"{uuid.uuid4()}.{ext}"
    return os.path.join(base_folder, str(instance.user.id), unique_name)


def layout_upload_path(instance, filename):
    return unique_file_path(instance, filename, "layouts")


class Level(models.Model):
    title = models.CharField(max_length=255)
    user = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    layout_file = models.FileField(upload_to=layout_upload_path)
    date_created = models.DateTimeField(auto_now_add=True)
    timer = models.IntegerField(null=True, blank=True) # in seconds
    difficulty = models.CharField(max_length=10)


