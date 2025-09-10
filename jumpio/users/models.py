from django.contrib.auth.models import AbstractUser
from django.db import models

class User(AbstractUser):
    date_joined = models.DateTimeField(auto_now_add=True)
