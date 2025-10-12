from django.urls import path
from rest_framework.routers import DefaultRouter
from .views import LevelViewSet

router = DefaultRouter()
router.register("levels", LevelViewSet, basename="levels")

urlpatterns = [
    *router.urls
]