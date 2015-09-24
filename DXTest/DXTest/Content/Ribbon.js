function OnFileUploadComplete(s, e) {
    ajaxUpdateOpenFilename();
    reloadXmlTreeView();
}
// When a button image on the Ribbon is clicked, this function is called
function OnRibbonItemClicked(s, e) {
    // The name of the image button that was clicked
    var name = e.item.name;

    var focusedNodeKey = treeList.GetFocusedNodeKey();

    if (name == "AddElement") {
        AddNewElement(focusedNodeKey);
    }
    else if (name == "AddAttribute") {
        AddNewAttribute(focusedNodeKey);
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
    else if (name == "Paste") {
        Paste();
    }
    else if (name == "Copy") {
        Copy();
    }
    else if (name == "Cut") {
        Cut();
    }
}
function loadFileFromDatabase() {
    var selectedValue = openFileList.GetSelectedItems()[0].text;
    ajaxOpenFileFromDatabase(selectedValue);
}
function onSelectionChanged(s, e) {
    var selectedValue = saveFileList.GetSelectedItems()[0].text;
    textBoxFileName.SetText(selectedValue);
}
function updateDatabaseFileLists() {
    saveFileList.PerformCallback();
    openFileList.PerformCallback();
}
function saveFileToDatabase() {
    ajaxSaveFileToDatabase(textBoxFileName.GetText(), false);
}
function ajaxUpdateOpenFilename() {
    $.ajax({
        type: "POST",
        url: "/XmlEditor/UpdateOpenFilename",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            if (response == "")
            {
                alert("Invalid XML file");
            }
            textBoxFileName.SetText(response);
        },
        error: function (response) {
            alert("An error occured");
        }
    });
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
            alert("An error occured");
        }
    });
}
function ajaxOpenFileFromDatabase(name) {
    var data = {
        filename: name
    }
    $.ajax({
        type: "POST",
        url: "/XmlEditor/OpenXmlFromDatabase",
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            ajaxUpdateOpenFilename()
            reloadXmlTreeView();
        },
        error: function (response) {
            alert("An error occured");
        }
    });
}
function ajaxSaveFileToDatabase(name, over) {
    var data = {
        filename: name,
        overwrite: over
    };

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
                // If the response is false, a file with that name already exists. Ask the user if he wants to overwrite the existing file
                var r = confirm("A file with that name already exists in the database. Would you like to overwrite it?");
                if (r == true) {
                    ajaxSaveFileToDatabase(textBoxFileName.GetText(), true);
                }
            }
        },
        error: function (response) {
            alert("An error occured");
        }
    });
}