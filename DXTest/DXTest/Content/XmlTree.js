function reloadXmlTreeView() {
    treeList.PerformCallback();
}
function NewElement() {
    var focusedNodeKey = treeList.GetFocusedNodeKey();
    AddNewElement(focusedNodeKey);
}
function NewAttribute() {
    var focusedNodeKey = treeList.GetFocusedNodeKey();
    AddNewAttribute(focusedNodeKey);
}
function Delete() {
    var focusedNodeKey = treeList.GetFocusedNodeKey();
    ajaxDeleteNode(focusedNodeKey);
}
function Copy() {
    var focusedNodeKey = treeList.GetFocusedNodeKey();
    ajaxCopy(focusedNodeKey);
}
function Paste() {
    var focusedNodeKey = treeList.GetFocusedNodeKey();
    ajaxPaste(focusedNodeKey);
}
function Cut() {
    var focusedNodeKey = treeList.GetFocusedNodeKey();
    ajaxCut(focusedNodeKey);
}
// Binds our custom context menu to the xml tree, so that when the user right clicks on the XML tree, the context menu shows up
function InitPopupMenuHandler(s, e) {
    //$("#xmlTree").bind("contextmenu", OnGridContextMenu);
}
// Displays our custom context menu for right clicks
function OnGridContextMenu(evt) {
    //PopupMenu.ShowAtPos(evt.pageX, evt.pageY);
    OnPreventContextMenu(evt);
}
// Prevents the default browser context menu from showing up when the user right clicks
function OnPreventContextMenu(evt) {
    evt.preventDefault();
}
// When the user right clicks in the TreeList this function is called
function OnTreeListRightClick(s, e) {
    treeList.SetFocusedNodeKey(e.objectKey);
    if (e.objectType == "Node") {
        // Show a popup menu at the point the user clicked
        PopupMenu.ShowAtPos(ASPxClientUtils.GetEventX(e.htmlEvent), ASPxClientUtils.GetEventY(e.htmlEvent));
    }
}
function OnPopupMenuItemClick(s, e) {
}
function collapseAll() {
    treeList.CollapseAll();
}
function expandAll() {
    treeList.ExpandAll();
}
function onTreeListCallBackError(s, e) {
    e.handled = true;
    alert("An error occured trying to update the node");
}
function AddNewElement(focusedNodeKey) {
    addNewNode(focusedNodeKey, 0);
}
function AddNewAttribute(focusedNodeKey) {
    addNewNode(focusedNodeKey, 1);
}
function makeNamespaceDeclaration(key) {
    ajaxMakeNamespaceDeclaration(key);
}
function addNewNode(focusedNodeKey, type) {
    var data = {
        focusedNode: focusedNodeKey,
        type: type
    };
    ajaxAddNewNode(data);
}
function ajaxCopy(node) {
    var data = { id: node };
    $.ajax({
        type: "POST",
        url: "/XmlEditor/Copy",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
        },
        error: function (response) {
            alert("An error occured");
        }
    });
}
function ajaxCut(node) {
    var data = { id: node };
    $.ajax({
        type: "POST",
        url: "/XmlEditor/Cut",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            reloadXmlTreeView();
        },
        error: function (response) {
            alert("An error occured");
        }
    });
}
function ajaxPaste(node) {
    var data = { id: node };
    $.ajax({
        type: "POST",
        url: "/XmlEditor/Paste",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            reloadXmlTreeView();
        },
        error: function (response) {
            alert("An error occured");
        }
    });
}
function ajaxDeleteNode(node) {
    var data = { id: node }
    $.ajax({
        type: "POST",
        url: "/XmlEditor/DeleteNodeJson",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            reloadXmlTreeView();
        },
        error: function (response) {
            alert("An error occured");
        }
    });
}
function ajaxAddNewNode(data) {
    $.ajax({
        type: "POST",
        url: "/XmlEditor/AddNode",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            reloadXmlTreeView();
        },
        error: function (response) {
            alert("An error occured");
        }
    });
}
function ajaxMakeNamespaceDeclaration(key) {
    var data = {
        focusedNodeKey: key
    };
    $.ajax({
        type: "POST",
        url: "/XmlEditor/MakeNamespaceDeclaration",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            reloadXmlTreeView();
        },
        error: function (response) {
            alert("An error occured");
        }
    });
}