
let showByStatus = 'all';

const filterContainer = document.querySelector('.filter-projects');
const filterByStatusBtn = document.querySelectorAll('[data-filter-status]')
filterByStatusBtn.forEach(button => {
    button.addEventListener('click', function () {
        const currentActive = filterContainer.querySelector('.active-filter');
        if (currentActive) {
            currentActive.classList.remove('active-filter');
        }

        this.parentElement.classList.add('active-filter');
        showByStatus = this.parentElement.id;

        getProjects();
    });
});

let debounceTimer;
document.getElementById("search-field").addEventListener("input", (e) => {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(() => {
    getProjects(e.target.value);
    }, 500);
});

async function getProjects(query = "") {
    const url = `/home/projects?query=${query}`;
    const data = await fetchData(url)

    const statusFilterdData = organizeAndCountProjetAfterStatus(data, showByStatus)

    const projectView = document.getElementById('projects-view')
    if (projectView) {
        projectView.innerHTML = "";
        for (let project of statusFilterdData) {
            let timeDiffClass = 'label'
            if (Date.now() > new Date(project.endDate).getTime()) timeDiffClass = 'label-red';
            if (project.status === 2) timeDiffClass = 'label-green';
            let card = projectCard(project.id, project.imageUrl, project.projectName, project.client.clientName, project.description, project.members, project.calculatedTimeDiff, timeDiffClass)
            projectView.insertAdjacentHTML('beforeend', card)
        }
    } 
}

function projectCard(id, imageUrl, projectName, clientName, description, members, timeDiff, timeDiffClass) {
    return `
<div class="project-card">
    <div>
        <img class="project-img" src="${imageUrl}" alt="project display image">
    </div>
    <div class="project-card-headings">
        <h6 class="project-name">${projectName}</h6>
        <p class="org-name">${clientName}</p>
    </div>
    <div class="project-card-settings">
        <div class="popover-container">
            <button data-open-pop-over data-targetId="#pop-over-${id}" ><i class="bi bi-three-dots"></i></button>
            <div id="pop-over-${id}" data-pop-over class="popover d-none">
                <div class="flex align c-gap-5">
                    <i class="bi bi-pencil-square"></i>
                    <button onclick="fetchPopulateProject('${id}')" data-targetId="#edit-project" data-openModal class="btn-clean">
                        Edit Project
                    </button>
                </div>
                <div class="flex align c-gap-5">
                    <i class="bi bi-check2-circle"></i>
                    <button onClick="sendDataAsQuery('/updateStatus/${id}?status=Completed', ${true})" class="btn-clean">
                        Mark As Completed
                    </button>
                </div>
                <div class="flex align c-gap-5 border-bottom">
                    <i class="bi bi-person-add"></i>
                    <button onclick="fetchPopulateProjectMembers('${id}')" data-targetId="#add-members" data-openModal class="btn-clean">
                        Add Members
                    </button>
                </div>
                <div class="flex align c-gap-5">
                    <i class="bi bi-trash text-error"></i>
                    <button onClick="deleteData('/deleteProject/${id}')" class="btn-clean text-error">
                        Delete
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="project-card-content">
        <p>${description}</p>
    </div>
    <div class="${timeDiffClass} project-countdown">
        <i class="bi bi-clock-fill"> <span>${timeDiff}</span></i>
    </div>
    <div class="project-members">
        ${members.map(member => (`<img class="profile-img-medium" src="${member.imageUrl}" />`)).join('')}
    </div>
</div>
        `};

function organizeAndCountProjetAfterStatus(data, filterBy) {
    let all = document.getElementById('count-all')
    let started = document.getElementById('count-started')
    let completed = document.getElementById('count-completed')

    if(!all) return
   
    all.innerText = data.length;
    const startedProjects = [...data.filter(x => x.status === 1)];
    const completedProjects = [...data.filter(x => x.status === 2)];


    started.innerText = startedProjects.length;
    completed.innerText = completedProjects.length

    if(filterBy === 'started') return startedProjects;
    if(filterBy === 'completed') return completedProjects;

    return data
    }

window.addEventListener('DOMContentLoaded', () => {
    getProjects();
})

async function fetchPopulateProject(projectId) {
    const url = `/project/${projectId}`
    const data = await fetchData(url);
    populateProjectForm(data)
}

function populateProjectForm(data) {
    const form = document.getElementById('edit-project-form');
    form.action = `updateProject/${data.id}`

    form.getElementsByTagName('img')[0].src = data['imageUrl']
    form.querySelector('input[name="ProjectForm.ImageUrl"]').value = data['imageUrl'];
    form.querySelector('input[name="ProjectForm.ProjectName"]').value = data['projectName'];
    form.querySelector('input[name="ProjectForm.ClientName"]').value = data['client'].clientName;
    form.querySelector('input[name="ProjectForm.StartDate"]').value = formatDate(data['startDate']);;
    form.querySelector('input[name="ProjectForm.EndDate"]').value = formatDate(data['endDate']);
    form.querySelector('input[name="ProjectForm.Budget"]').value = data['budget'];

    initQuill('#edit-project-quill-editor', '#edit-project-quill-toolbar', '#edit-project-textarea', data['description'])

    let members = data['members']
    const options = form.getElementsByTagName('option')
    for (let option of options) {
        for (let member of members) {
            if (member.id === option.value) {
                option.setAttribute("selected", "");
            }
        }
    }
    createInitialTokens(form.getElementsByTagName('select')[0])
} 

async function fetchPopulateProjectMembers(projectId) {
    const url = `https://localhost:8081/project/${projectId}`
    const data = await fetchData(url);
    populateProjectMembersForm(data)
}

function populateProjectMembersForm(data) {
    const form = document.getElementById('add-members-form');
    form.action = `updateProjectMembers/${data.id}`

    let members = data['members']
    const options = form.getElementsByTagName('option')
    for (let option of options) {
        for (let member of members) {
            if (member.id === option.value) {
                console.log(member)
                option.setAttribute("selected", "");
            }
        }
    }
    createInitialTokens(form.getElementsByTagName('select')[0])
}