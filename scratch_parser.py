
import re

def parse_unity_scene(filepath):
    # Unity YAML is not strictly standard, so we do a quick text parse
    objects = {}
    current_go = None
    current_transform = None
    
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
        
    # Find GameObjects
    go_matches = re.finditer(r'--- !u!1 &(\d+)\nGameObject:\n(.*?)(?=\n---|\Z)', content, re.DOTALL)
    for match in go_matches:
        file_id = match.group(1)
        go_data = match.group(2)
        name_match = re.search(r'm_Name: (.*)', go_data)
        name = name_match.group(1).strip() if name_match else "Unknown"
        objects[file_id] = {'name': name, 'transform_id': None, 'children': []}
        
    # Find Transforms
    tr_matches = re.finditer(r'--- !u!4 &(\d+)\n(?:Rect)?Transform:\n(.*?)(?=\n---|\Z)', content, re.DOTALL)
    transforms = {}
    for match in tr_matches:
        file_id = match.group(1)
        tr_data = match.group(2)
        
        go_match = re.search(r'm_GameObject:\n\s+fileID: (\d+)', tr_data)
        parent_match = re.search(r'm_Father:\n\s+fileID: (\d+)', tr_data)
        
        go_id = go_match.group(1) if go_match else None
        parent_tr_id = parent_match.group(1) if parent_match else None
        
        if parent_tr_id == "0":
            parent_tr_id = None
            
        transforms[file_id] = {'go_id': go_id, 'parent_tr_id': parent_tr_id, 'children_tr': []}
        
        # also find children
        children_section = re.search(r'm_Children:\n((?:\s+- fileID: \d+\n)*)', tr_data)
        if children_section:
            child_matches = re.findall(r'- fileID: (\d+)', children_section.group(1))
            transforms[file_id]['children_tr'] = child_matches
            
        if go_id and go_id in objects:
            objects[go_id]['transform_id'] = file_id
            
    # Build tree
    root_gos = []
    
    for tr_id, tr_data in transforms.items():
        go_id = tr_data['go_id']
        parent_tr_id = tr_data['parent_tr_id']
        
        if not go_id or go_id not in objects:
            continue
            
        if parent_tr_id and parent_tr_id in transforms:
            parent_go_id = transforms[parent_tr_id]['go_id']
            if parent_go_id and parent_go_id in objects:
                objects[parent_go_id]['children'].append(go_id)
        else:
            root_gos.append(go_id)
            
    def print_tree(g_id, indent=""):
        if g_id not in objects: return
        print(f"{indent}- {objects[g_id]['name']}")
        
        # Sort children to maintain stable output (optional)
        for child_id in objects[g_id]['children']:
            print_tree(child_id, indent + "  ")

    print("=== MagnetSetup & ControlsSetup Hierarchy ===")
    for g_id in root_gos:
        name = objects[g_id]['name']
        if name in ['MagnetSetup', 'ControlsSetup']:
            print_tree(g_id)

parse_unity_scene('Assets/Challenges/03_Training/Scenes/Training_Prototype_Broken.unity')
