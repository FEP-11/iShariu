document.addEventListener('DOMContentLoaded', (event) => {
    document.getElementById('submitChangePassword').addEventListener('click', function(event) {

        event.preventDefault();

        var currentPassword = document.getElementById('currentPassword').value;
        var newPassword = document.getElementById('newPassword').value;

        fetch('/account/changepassword', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                currentPassword: currentPassword,
                newPassword: newPassword
            }),
        })
            .then(response => {
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    document.getElementById('changePasswordModal').style.display = 'none';
                    document.querySelector('.modal-backdrop').remove();

                    var successToast = new bootstrap.Toast(document.getElementById('successToast'));
                    successToast.show();
                } else {
                    document.getElementById('errorMessage').textContent = 'Error: ' + data.message;
                }
            })
            .catch((error) => {
                console.error('Error:', error);
            });
    });
});

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

document.getElementById('revealPhoneNumber').addEventListener('click', function() {
    var phoneNumberDisplay = document.getElementById('phoneNumberDisplay');
    var phoneNumber = phoneNumberDisplay.getAttribute('data-phone-number');
    var button = this;

    if (button.textContent.toLowerCase() === 'reveal') {
        phoneNumberDisplay.textContent = "Your current phone number is: " + phoneNumber + ".";
        button.textContent = 'Hide';
    } else {
        phoneNumberDisplay.textContent = "Your current phone number is: ******" + phoneNumber.slice(-4) + ".";
        button.textContent = 'Reveal';
    }
});
document.getElementById('ageRestrictedContentToggle').addEventListener('change', function() {
    updateSetting('AllowAccessToAgeRestrictedContent', this.checked);
});

document.getElementById('improveIShariuToggle').addEventListener('change', function() {
    updateSetting('UseDataToImproveIShariu', this.checked);
});

function updateSetting(settingName, settingValue) {
    fetch('/account/updatesetting', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            settingName: settingName,
            settingValue: settingValue
        }),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}

document.getElementById('confirmDeleteAccount').addEventListener('click', function() {
    var password = document.getElementById('deleteAccountPassword').value;
    var url = '/account/deleteaccount';
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            password: password
        }),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if (data.success === false) {
                document.getElementById('errorMessage').textContent = 'Error: ' + data.message;
            } else {
                window.location.href = '/account/signin';
            }
        })
        .catch((error) => {
            console.error('Error:', error);
        });
});
