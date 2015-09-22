function reloadXmlTreeView() {
    treeList.PerformCallback();
}
function onTreeListCallBackError(s, e) {
    e.handled = true;
    alert("The updated data is invalid");
}
function AddNewElement(focusedNodeKey) {
    addNewNode(focusedNodeKey, 0);
}
function AddNewAttribute(focusedNodeKey) {
    addNewNode(focusedNodeKey, 1);
}
function collapseAll() {
    treeList.CollapseAll();
}
function expandAll() {
    treeList.ExpandAll();
}
function makeNamespaceDeclaration(key) {
    var data = {
        focusedNodeKey: key
    };
    ajaxMakeNamespaceDeclaration(data);
}
function addNewNode(focusedNodeKey, type) {
    var data = {
        focusedNode: focusedNodeKey,
        type: type
    };
    ajaxAddNewNode(data);
}