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
    var data = { id: focusedNodeKey }
    ajaxDeleteNode(data);
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
function OnTreeListRightClick(s, e) {
    treeList.SetFocusedNodeKey(e.objectKey);

    if (e.objectType == "Node") {
        ///* prepare popup menu */
        //var state = tree.GetNodeState(e.objectKey);

        //popupMenu.GetItem(0).SetEnabled(state != "Child" && state != "NotFound");
        //popupMenu.GetItem(1).SetEnabled(state == "Child" || state == "NotFound");

        PopupMenu.ShowAtPos(ASPxClientUtils.GetEventX(e.htmlEvent), ASPxClientUtils.GetEventY(e.htmlEvent));
    }
}
function OnPopupMenuItemClick(s, e) {
    //alert("HEJ");
    //if (e.item == null || e.item.name == $("#CheckedItemName").val())
    //    return;
    //$("#CheckedItemName").val(e.item.name);
    //grid.PerformCallback({ "sortColumn": e.item.name });
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
        }
    });
}
function ajaxDeleteNode(data) {
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
        }
    });
}