function showAlert(title, message, icon = 'info') {
    Swal.fire({
        title: title,
        text: message,
        icon: icon,
        confirmButtonText: 'OK'
    });
}

function showSuccessAlert(message) {
    showAlert('Success!', message, 'success');
}

function showErrorAlert(message) {
    showAlert('Error!', message, 'error');
}

function showWarningAlert(message) {
    showAlert('Warning!', message, 'warning');
}

function showCustomAlert(title, message, icon, confirmButtonText, callback) {
    Swal.fire({
        title: title,
        text: message,
        icon: icon,
        confirmButtonText: confirmButtonText
    }).then((result) => {
        if (result.isConfirmed && callback) {
            callback();
        }
    });
}
