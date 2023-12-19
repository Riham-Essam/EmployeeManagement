function confirmDelete(uniqueID, isDeletedClicked) {

    var deleleUser = 'Delete_' + uniqueID;
    var confirmDelete = 'ConfirmDelete_' + uniqueID;

    if (isDeletedClicked) {

        $('#' + deleleUser).hide();
        $('#' + confirmDelete).show();

    } else {

        $('#' + deleleUser).show();
        $('#' + confirmDelete).hide();

    }
}