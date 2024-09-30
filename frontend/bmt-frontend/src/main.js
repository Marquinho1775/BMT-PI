import { createApp } from 'vue';
import App from './App.vue';
import { createRouter, createWebHistory } from 'vue-router';
// import HomePage from './components/HomePage.vue';
import HomePageUserClient from './components/HomePageUserClient.vue';
import HomePageEntrepeneur from './components/HomePageEntrepeneur.vue';
import RegisterForm from './components/RegisterForm.vue';
import LoginForm from './components/LoginForm.vue';
import EnterpriseRegisterForm from './components/EnterpriseRegisterForm.vue';
import EntrepreneurRegisteredEnterprises from './components/EntrepreneurRegisteredEnterprises.vue';
import EmailVerification from './components/EmailVerification.vue';
import HomePageGuest from './components/HomePageGuest.vue';
import HomePageDeveloper from './components/HomePageDeveloper.vue';
import DeveloperEnterprises from './components/DeveloperEnterprises.vue';
import DeveloperProducts from './components/DeveloperProducts.vue';
import DeveloperUsers from './components/DeveloperUsers.vue';
import ProfilePage from './components/ProfilePage.vue';
import HomePageGuest from './components/HomePageGuest.vue';
import CollaboratorProfilePage from './components/CollaboratorProfilePage.vue';
import RegisterAddressForm from './components/RegisterAddress.vue';

// Import Bootstrap and BootstrapVue
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue-3/dist/bootstrap-vue-3.css';

// Import BootstrapVue3
import BootstrapVue3 from 'bootstrap-vue-3';

// Import SweetAlert2
import Swal from 'sweetalert2';

// Import the authentication utilities
import { getToken } from './helpers/auth';
import axios from 'axios';

// Set up Axios interceptors
axios.interceptors.request.use(config => {
    const token = getToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, error => {
    return Promise.reject(error);
});

const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', name: "Home", component: HomePageGuest },
        { path: '/clienthome', name: "ClientHome", component: HomePageUserClient },
        { path: '/entrepeneurhome', name: "entrepeneurhome", component: HomePageEntrepeneur },
        { path: '/register', name: "Register", component: RegisterForm },
        { path: '/login', name: "Login", component: LoginForm },
        { path: '/enterprise-register', name: "EnterpriseRegister", component: EnterpriseRegisterForm },
        { path: '/email-verification', name: "VerifyEmail", component: EmailVerification },
        { path: '/login', name: "Login", component: LoginForm },
        { path: '/developerhome', name: "DeveloperHome", component: HomePageDeveloper},
        { path: '/developer-enterprises', name: "DeveloperEnterprises", component: DeveloperEnterprises},
        { path: '/developer-products', name: "DeveloperProducts", component: DeveloperProducts},
        { path: '/developer-users', name: "DeveloperUsers", component: DeveloperUsers}
        { path: '/enterprises', name: 'EntrepreneurRegisteredEnterprises', component: EntrepreneurRegisteredEnterprises },
        { path: '/email-verification', name: "VerifyEmail", component: EmailVerification },
        { path: '/login', name: "Login", component: LoginForm },
        { path: '/profile', name: "Profile", component: ProfilePage },
        { path: '/collaborator', name: "CollaboratorProfile", component: CollaboratorProfilePage },
        { path: '/register-address', name: "RegisterAddress", component: RegisterAddressForm }
    ]
});

// Add route guard to protect routes that require authentication
router.beforeEach((to, from, next) => {
    const token = getToken();

    // Redirect to login if the route requires authentication and no token is found
    if (to.meta.requiresAuth && !token) {
        next('/login');
    } else {
        next();
    }
});

const app = createApp(App);

// Use BootstrapVue3
app.use(BootstrapVue3);

// Use the router
app.use(router);

// Add SweetAlert2 globally
app.config.globalProperties.$swal = Swal;

// Mount the app
app.mount('#app');