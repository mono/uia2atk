#include "mytk.h"

static GList* top_level_windows = NULL;

void
mytk_add_one_top_level_window(gchar* name)
{
  //no need to check for NULL as append() already mallocs
  top_level_windows = g_list_append(top_level_windows, (gpointer)mytk_widget_new(name));
}

GList* mytk_window_list_toplevels ()
{
	return top_level_windows;
}
