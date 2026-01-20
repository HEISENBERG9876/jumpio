from rest_framework.routers import DefaultRouter
from .views import CampaignViewSet

router = DefaultRouter()
router.register("campaigns", CampaignViewSet, basename="campaigns")

urlpatterns = router.urls