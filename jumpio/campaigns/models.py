import uuid
from django.db import models
from django.conf import settings


class Campaign(models.Model):
    title = models.CharField(max_length=255)
    user = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE, related_name="campaigns")
    description = models.TextField(blank=True, null=True)
    date_created = models.DateTimeField(auto_now_add=True)

    def __str__(self):
        return self.title


class CampaignLevel(models.Model):
    campaign = models.ForeignKey(Campaign, on_delete=models.CASCADE, related_name="campaign_levels")
    level = models.ForeignKey("levels.Level", on_delete=models.CASCADE, related_name="in_campaigns")
    order_index = models.PositiveIntegerField()

    class Meta:
        constraints = [
            models.UniqueConstraint(fields=["campaign", "order_index"], name="uniq_campaign_order_index"),
            models.UniqueConstraint(fields=["campaign", "level"], name="uniq_campaign_level"),
        ]
        ordering = ["order_index"]

    def __str__(self):
        return f"{self.campaign}" # todo
