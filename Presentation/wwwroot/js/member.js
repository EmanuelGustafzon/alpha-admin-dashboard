let debounceMemberTimer;
document.getElementById("search-field").addEventListener("input", (e) => {
    clearTimeout(debounceMemberTimer);
    debounceMemberTimer = setTimeout(() => {
        getMembers(e.target.value);
    }, 500);
});

async function getMembers(query = "") {
    const url = `/members/allMembers?query=${query}`;
    const data = await fetchData(url)

    const membersView = document.getElementById('members-view')
    membersView.innerHTML = "";
    for (const member of data) {
        const card = memberCard(member.id, member.imageUrl, member.firstName, member.lastName, member.jobTitle, member.phoneNumber, member.email)
        membersView.insertAdjacentHTML('beforeend', card)
    }
}

function memberCard(id, imageUrl, firstName, lastName, jobTitle, phone, email) {
    return `
            <div class="profile-card">
                <button onclick="fetchPopulateMember('${id}')" data-targetId="#edit-member" data-openModal class="flex flex-end btn-clean">
                    <i class="bi bi-three-dots"></i>
                </button>
                <img src="${imageUrl ? imageUrl : '/images/default-profile-picture.png'}" alt="member profile picture">
                <h4>${firstName} ${lastName}</h4>
                <div class="member-job-title">
                    <p>${jobTitle}</p>
                </div>
                <p>${phone}</p>
                <p>${email}</p>
                <button class="btn btn-secondary">Message</button>
            </div>
        `;
}
window.addEventListener('DOMContentLoaded', () => {
    getMembers();
})

async function fetchPopulateMember(memberId) {
    const url = `/member/${memberId}`
    const data = await fetchData(url);
    console.log(data)
    populateMemberForm(data)
}

async function populateMemberForm(data) {
    const form = document.getElementById('edit-member-form');

    form.action = `updateMember/${data.id}`

    const memberUseExternalProvider = await fetchData(`/memberUseExternalProvider/${data.id}`)

    form.getElementsByTagName('img')[0].src = data['imageUrl']
    form.querySelector('input[name="MemberWithRoleForm.ImageUrl"]').value = data['imageUrl'];
    form.querySelector('input[name="MemberWithRoleForm.FirstName"]').value = data['firstName'];
    form.querySelector('input[name="MemberWithRoleForm.LastName"]').value = data['lastName'];
    const email = form.querySelector('input[name="MemberWithRoleForm.Email"]')
    if (memberUseExternalProvider.message === true) {
        email.parentNode.hidden = true

    } else {
        email.value = data['email'];
        email.parentNode.hidden = false
    }
    form.querySelector('input[name="MemberWithRoleForm.PhoneNumber"]').value = data['phoneNumber'];
    form.querySelector('input[name="MemberWithRoleForm.JobTitle"]').value = data['jobTitle'];
    form.querySelector('select[name="MemberWithRoleForm.Role"]').value = data['role']
    form.querySelector('input[name="MemberWithRoleForm.BirthDate"]').value = formatDate(data['birthDate']);
    form.querySelector('input[name="MemberWithRoleForm.AddressForm.City"]').value = data['address'].city;
    form.querySelector('input[name="MemberWithRoleForm.AddressForm.PostCode"]').value = data['address'].postCode;
    form.querySelector('input[name="MemberWithRoleForm.AddressForm.Street"]').value = data['address'].street;
}