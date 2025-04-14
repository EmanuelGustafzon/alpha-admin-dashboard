﻿//send forms with fetch
const forms = document.querySelectorAll("form");
forms.forEach(form => {
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        clearErrorMesseges(form);

        const formData = new FormData(form);
        try {
            const res = await fetch(form.action, {
                method: 'post',
                body: formData
            });
            if (res.ok) window.location.reload();
            
            if (res.status == 400) {
                const data = await res.json();
                if (data.errors) {
                    Object.keys(data.errors).forEach(key => {
                        const input = form.querySelector(`[name="${key}"]`);
                        if (input)
                            input.classList.add('input-error');
                        const span = form.querySelector(`[data-valmsg-for="${key}"]`)
                        if (span) {
                            span.innerText = data.errors[key].join('\n');
                            span.classList.add('text-error');
                        }
                    })
                }
            }
        } catch (error) {
            console.log("Error submitting form", error)
        }
    })
})
function clearErrorMesseges(form) {
    form.querySelectorAll('[data-val="true"]').forEach(input => {
        input.classList.remove("input-error");

    })
    form.querySelectorAll('[data-valmsg-for]').forEach(span => {
        span.innerText = "";
        span.classList.remove('text-error')
    })
}
// popovers
document.addEventListener('click', function (e) {
    const toggleBtn = e.target.closest('[data-open-pop-over]');

    // Close all popovers if click is outside
    if (!e.target.closest('[data-pop-over]')) document.querySelectorAll('[data-pop-over]').forEach(p => p.classList.add('d-none'));

    if (!toggleBtn) return;

    const targetId = toggleBtn.getAttribute('data-targetId');
    const popover = document.querySelector(targetId);
    if (popover) {
        popover.classList.toggle('d-none');
    }
});

// handle open and close of form modals 
document.addEventListener('click', function (e) {
    const openModalBtn = e.target.closest('[data-openModal="true"]');

    if (!openModalBtn) return;

    const targetId = openModalBtn.getAttribute('data-targetId');
    const modal = document.querySelector(targetId);
    if (modal) {
        modal.classList.remove('d-none');
    }
});

const closeModalButtons = document.querySelectorAll('[data-closeModal="true"]');
closeModalButtons.forEach(btn => {
    btn.addEventListener("click", () => {
        const modal = btn.closest('.form-modal');
        if (!modal) {
            console.error("Modal element not found!");
            return;
        }
        modal.classList.add("d-none")
        const forms = modal.querySelectorAll('form')
        forms.forEach(form => form.reset());
        forms.forEach(form => clearErrorMesseges(form));
        
        const imgPreview = modal.querySelector('.img-preview')
        if (imgPreview)
            imgPreview.src = "";

        const imgPreviewContainer = document.querySelector(".img-preview-container")
        if (imgPreviewContainer)
            imgPreviewContainer.classList.remove("selected");
    })
})

// handle image previews 
document.querySelectorAll('.img-preview-container').forEach(previewContainer => {
    const fileInput = previewContainer.querySelector('input[type="file"]')
    const imagePreview = previewContainer.querySelector('.img-preview')

    previewContainer.addEventListener('click', () => fileInput.click());

    fileInput.addEventListener('change', ({ target: { files } }) => {
        const file = files[0];
        if (file) {
            const success = displayImage(file, imagePreview);
            if(success)
                previewContainer.classList.add('selected');
        }
    })
})
const readImageFile = (imageFile) => {
    return new Promise((resolve, reject) => {
        if (!imageFile.type.startsWith("image")) {
            return reject(new Error("The file is not an image"));
        }
        const fileReader = new FileReader();
        fileReader.onload = (e) => {
            const image = new Image();
            image.onerror = () => reject(new Error("could not read file"));
            image.src = e.target.result;
            resolve(image);
        }
        fileReader.onerror = (error) => {
            reject(new Error("could not read file"));
        };
        fileReader.readAsDataURL(imageFile);
    })
}
async function displayImage(file, imagePreview, size = 150) {
    try {
        const image = await readImageFile(file);
        const canvas = document.createElement('canvas');
        canvas.width = size;
        canvas.height = size;

        const context = canvas.getContext('2d');
        context.drawImage(image, 0, 0, size, size);
        imagePreview.src = canvas.toDataURL('image/jpeg');
        return true;
    } catch (error) {
        console.error('Failed to display image', error);
        return false;
    }
}

/*quill*/
function initQuill(editorId, toolbarId, textareaId, content) {
    const textarea = document.querySelector(textareaId);

    const quill = new Quill(editorId, {
        modules: {
            syntax: true,
            toolbar: toolbarId
        },
        theme: 'snow'
    });

    if (content) quill.root.innerHTML = content;

    quill.on('text-change', () => textarea.value = quill.root.innerHTML)
};

/*1. fetch data from url*/

async function fetchData(url) {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error(error.message);
    }
}

/* 2 populate view */



const formatDate = (datetime) => {
    return new Date(datetime).toISOString().split('T')[0];
};  
