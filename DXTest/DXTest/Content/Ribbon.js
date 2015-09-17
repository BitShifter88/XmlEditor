function reloadXmlTreeView() {
    treeList.PerformCallback();
}
function onTreeListCallBackError(s, e)
{
    e.handled = true;
    alert("The updated data is invalid");
}
// When a button image on the Ribbon is clicked, this function is called
function OnRibbonItemClicked(s, e) {
    // The name of the image button that was clicked
    var name = e.item.name;

    var focusedNodeKey = treeList.GetFocusedNodeKey();

    if (name == "AddElement") {
        addNewNode(focusedNodeKey, 0);
    }
    else if (name == "AddAttribute") {
        addNewNode(focusedNodeKey, 1);
    }
    else if (name == "MakeNamespaceDeclaration") {
        makeNamespaceDeclaration(focusedNodeKey);
    }
    else if (name == "ExpandAll")
    {
        expandAll();
    }
    else if (name == "CollapseAll") {
        collapseAll();
    }
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
function onSelectionChanged(s, e) {

    var selectedValue = saveFileList.GetSelectedItems()[0].text;
    textBoxFileName.SetText(selectedValue);

}
function loadFileFromDatabase() {
    var selectedValue = openFileList.GetSelectedItems()[0].text;
    var data = {
        filename: selectedValue
    }
    ajaxOpenFileFromDatabase(data);
}
function updateDatabaseFileLists() {
    saveFileList.PerformCallback();
    openFileList.PerformCallback();
}
function saveFileToDatabase() {

    var data = {
        filename: textBoxFileName.GetText(),
        overwrite: false
    };
    ajaxSaveFileToDatabase(data);
}
function ajaxNewDocument() {
    $.ajax({
        type: "POST",
        url: "/XmlEditor/NewDocument",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            reloadXmlTreeView();
        },
        error: function (response) {
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
        }
    });
}
function ajaxMakeNamespaceDeclaration(data) {
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
        }
    });
}
function ajaxOpenFileFromDatabase(data) {
    $.ajax({
        type: "POST",
        url: "/XmlEditor/OpenXmlFromDatabase",
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
function ajaxSaveFileToDatabase(data) {
    $.ajax({
        type: "POST",
        url: "/XmlEditor/SaveXmlToDatabase",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            if (response) {
                updateDatabaseFileLists();
            }
            else {
                var r = confirm("A file with that name already exists in the database. Would you like to overwrite it?");
                if (r == true) {
                    var d = {
                        filename: textBoxFileName.GetText(),
                        overwrite: true
                    };
                    ajaxSaveFileToDatabase(d);
                }
            }
        },
        error: function (response) {
            alert("error");
        }
    });
}