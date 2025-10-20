from django.contrib import admin

from .models import Level

@admin.register(Level)
class LevelAdmin(admin.ModelAdmin):
    list_display = ("id", "title", "user", "layout_file", "difficulty", "timer", "date_created")
