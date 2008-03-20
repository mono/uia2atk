#include "mytk.h"

static GList* top_level_windows = NULL;

void
mytk_add_one_top_level_window(gchar* name)
{
  if (!top_level_windows)
  {
    top_level_windows = g_list_alloc();
  }
  top_level_windows = g_list_append(top_level_windows, (gpointer)mytk_widget_new(name));
}

GList* mytk_window_list_toplevels ()
{
	return top_level_windows;
}
