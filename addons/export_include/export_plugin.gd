@tool

extends EditorExportPlugin

 # You could make these into project settings
var include_dirs = []
var include_files = ["res://fnky_parser.dll"]

var output_root_dir

func _get_name():
	return "export_include_plugin"

func _export_begin(features, is_debug, path, flags):
	output_root_dir = path.get_base_dir()

func _export_file(path, type, features):
	for file in include_files:
		if path == file:
			_export_file_our_way(path)
			return
	for dir in include_dirs:
		dir = dir.rstrip("/")
		if path.begins_with(dir) and (len(path) == len(dir) || path[len(dir)] == "/"):
			_export_file_our_way(path)
			return

func _export_file_our_way(path):
	# EditorExportPlugin function that tells it not to export the file
	skip()

	# Copy to the output directory

	var rfile = FileAccess.open(path, FileAccess.READ)
	var buffer = rfile.get_buffer(rfile.get_length())
	rfile.close()

	var path_suffix = path.trim_prefix("res://")
	var output_path = output_root_dir.path_join(path_suffix)
	var output_dir = output_path.get_base_dir().trim_prefix(output_root_dir)

	var dir = DirAccess.open(output_root_dir)
	if not dir.dir_exists(output_dir):
		dir.make_dir_recursive(output_dir)
	
	var wfile = FileAccess.open(output_path, FileAccess.WRITE)
	wfile.store_buffer(buffer)
	wfile.close()
