function confirmDelete(uniqueID, isDeletedClicked) {

    var deleteSpan = 'Delete_' + uniqueID;
    var confirmDelete = 'ConfirmDelete_' + uniqueID;

    if (isDeletedClicked) {

        $('#' + deleteSpan).hide();
        $('#' + confirmDelete).show();

    } else {

        $('#' + deleteSpan).show();
        $('#' + confirmDelete).hide();

    }
}