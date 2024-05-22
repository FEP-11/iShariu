document.addEventListener('DOMContentLoaded', function() {
    var accessElement = document.getElementById('access');
    var priceFieldElement = document.getElementById('priceField');
    var courseFormElement = document.getElementById('courseForm');
    var imageElement = document.getElementById('image');
    var priceElement = document.getElementById('price');
    var nextStepBtn = document.getElementById('nextStepBtn');
    var backStepBtn = document.getElementById('backStepBtn');
    var lessonContainer = document.getElementById('lessonContainer');
    var lessonIndex = 0;

    console.log('DOMContentLoaded event fired');


    if (accessElement && priceFieldElement) {
        console.log('accessElement and priceFieldElement found');
        accessElement.addEventListener('change', function() {
            console.log('accessElement change event fired');

            priceFieldElement.style.display = (this.value === 'Paid') ? 'block' : 'none';

            priceElement.required = (this.value === 'Paid');
        });
    } else {
        console.log('accessElement or priceFieldElement not found');
    }


    if (courseFormElement && imageElement) {
        console.log('courseFormElement and imageElement found');
        courseFormElement.addEventListener('submit', function(event) {
            console.log('courseFormElement submit event fired');
            var imageUrl = imageElement.value;
            console.log('imageUrl:', imageUrl);
            var urlPattern = new RegExp('^(https?:\\/\\/)?'+
                '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|'+
                '((\\d{1,3}\\.){3}\\d{1,3}))'+
                '(\\:\\d+)?'+
                '(\\/[-a-z\\d%@_.~+&:]*)*'+
                '(\\?[;&a-z\\d%@_.,~+&:=-]*)?'+
                '(\\#[-a-z\\d_]*)?$','i');
            console.log('urlPattern:', urlPattern);
            if (!imageUrl.match(urlPattern)) {
                console.log('imageUrl does not match urlPattern');
                alert('Please enter a valid URL for the course image.');
                event.preventDefault();
            } else {
                console.log('imageUrl matches urlPattern');
                var imageFormatPattern = /\.(jpg|png)$/i;
                if (!imageUrl.match(imageFormatPattern)) {
                    console.log('imageUrl does not end with .jpg or .png');
                    alert('Please enter a URL for the course image that ends with .jpg or .png.');
                    event.preventDefault();
                } else {
                    console.log('imageUrl ends with .jpg or .png');
                }
            }
        });
    } else {
        console.log('courseFormElement or imageElement not found');
    }


    if (nextStepBtn && backStepBtn) {
        nextStepBtn.addEventListener('click', function() {

            if (!validateStep1()) {
                alert('Please fill in all required fields.');
                return;
            }

            var step1 = document.querySelector('.step-1');
            var step2 = document.querySelector('.step-2');
            step1.style.display = 'none';
            step2.style.display = 'block';

            document.getElementById('step1Title').textContent = 'Create Lesson';

            var existingLesson = document.querySelector('.lesson-group');
            if (!existingLesson) {

                addLesson();
            }
        });


        backStepBtn.addEventListener('click', function() {
            var step1 = document.querySelector('.step-1');
            var step2 = document.querySelector('.step-2');
            step2.style.display = 'none';
            step1.style.display = 'block';

            document.getElementById('step1Title').textContent = 'Create Course';
        });
    }


    function validateStep1() {
        var inputs = document.querySelectorAll('.step-1 input[required], .step-1 textarea[required], .step-1 select[required]');
        for (var i = 0; i < inputs.length; i++) {
            if (!inputs[i].value) {
                return false;
            }
        }
        return true;
    }


    function addLesson() {
        if (lessonIndex < 10) {
            var lessonGroup = document.createElement('div');
            lessonGroup.className = 'form-group lesson-group';

            var nameLabel = document.createElement('label');
            nameLabel.for = 'lessons[' + lessonIndex + '].Name';
            nameLabel.textContent = 'Lesson Name';
            lessonGroup.appendChild(nameLabel);

            var nameInput = document.createElement('input');
            nameInput.type = 'text';
            nameInput.className = 'form-control';
            nameInput.name = 'lessons[' + lessonIndex + '].Name';
            lessonGroup.appendChild(nameInput);

            var urlLabel = document.createElement('label');
            urlLabel.for = 'lessons[' + lessonIndex + '].VideoUrl';
            urlLabel.textContent = 'Video URL';
            lessonGroup.appendChild(urlLabel);

            var urlInput = document.createElement('input');
            urlInput.type = 'text';
            urlInput.className = 'form-control';
            urlInput.name = 'lessons[' + lessonIndex + '].VideoUrl';
            lessonGroup.appendChild(urlInput);

            var contentLabel = document.createElement('label');
            contentLabel.for = 'lessons[' + lessonIndex + '].TextContent';
            contentLabel.textContent = 'Text Content';
            lessonGroup.appendChild(contentLabel);

            var contentInput = document.createElement('textarea');
            contentInput.className = 'form-control';
            contentInput.name = 'lessons[' + lessonIndex + '].TextContent';
            lessonGroup.appendChild(contentInput);

            lessonContainer.appendChild(lessonGroup);
            lessonIndex++;
        }
    }
});


document.addEventListener('DOMContentLoaded', function() {
    var addLessonBtn = document.getElementById('addLessonBtn');
    var lessonContainer = document.getElementById('lessonContainer');
    var lessonIndex = 0;

    addLessonBtn.addEventListener('click', function(e) {
        e.preventDefault();

        var lessonGroups = lessonContainer.getElementsByClassName('lesson-group');
        if (lessonGroups.length < 10) {
            var lessonGroup = document.createElement('div');
            lessonGroup.className = 'form-group lesson-group';

            var nameLabel = document.createElement('label');
            nameLabel.for = 'lessons[' + lessonIndex + '].Name';
            nameLabel.textContent = 'Lesson Name';
            lessonGroup.appendChild(nameLabel);

            var nameInput = document.createElement('input');
            nameInput.type = 'text';
            nameInput.className = 'form-control';
            nameInput.name = 'lessons[' + lessonIndex + '].Name';
            lessonGroup.appendChild(nameInput);

            var urlLabel = document.createElement('label');
            urlLabel.for = 'lessons[' + lessonIndex + '].VideoUrl';
            urlLabel.textContent = 'Video URL';
            lessonGroup.appendChild(urlLabel);

            var urlInput = document.createElement('input');
            urlInput.type = 'text';
            urlInput.className = 'form-control';
            urlInput.name = 'lessons[' + lessonIndex + '].VideoUrl';
            lessonGroup.appendChild(urlInput);

            var contentLabel = document.createElement('label');
            contentLabel.for = 'lessons[' + lessonIndex + '].TextContent';
            contentLabel.textContent = 'Text Content';
            lessonGroup.appendChild(contentLabel);

            var contentInput = document.createElement('textarea');
            contentInput.className = 'form-control';
            contentInput.name = 'lessons[' + lessonIndex + '].TextContent';
            lessonGroup.appendChild(contentInput);

            lessonContainer.appendChild(lessonGroup);
            lessonIndex++;
        }
    });
});

document.addEventListener('DOMContentLoaded', (event) => {
    var difficultySlider = document.getElementById('difficulty');
    var difficultyLabel = document.getElementById('difficultyLabel');

    difficultySlider.addEventListener('input', function() {
        difficultyLabel.textContent = 'Selected Difficulty: ' + this.value;
    });

    difficultyLabel.textContent = 'Selected Difficulty: ' + difficultySlider.value;
});