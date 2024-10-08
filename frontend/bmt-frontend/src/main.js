import { createApp } from 'vue';
import App from './App.vue';
import { createRouter, createWebHistory } from 'vue-router';
import HomePage from './components/HomePage.vue';
import HomePageUserClient from './components/HomePageUserClient.vue';
import HomePageEntrepeneur from './components/HomePageEntrepeneur.vue';

import RegisterForm from './components/RegisterForm.vue';
import LoginForm from './components/LoginForm.vue';
import EmailVerification from './components/EmailVerification.vue';
import ProfilePage from './components/ProfilePage.vue';

import EnterpriseRegisterForm from './components/EnterpriseRegisterForm.vue';
import EntrepreneurRegisteredEnterprises from './components/EntrepreneurRegisteredEnterprises.vue';
import CollaboratorProfilePage from './components/CollaboratorProfilePage.vue';

import HomePageDeveloper from './components/HomePageDeveloper.vue';
import DeveloperEnterprises from './components/DeveloperEnterprises.vue';
import DeveloperProducts from './components/DeveloperProducts.vue';
import DeveloperUsers from './components/DeveloperUsers.vue';
import RegisterAddressForm from './components/RegisterAddress.vue';
import EnterpriseDashboard from './components/EnterpriseDashboard.vue';
import ProductRegisterForm from './components/ProductRegisterForm.vue';

import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue-3/dist/bootstrap-vue-3.css';

import BootstrapVue3 from 'bootstrap-vue-3';
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
        { path: '/', name: "Home", component: HomePage },
        { path: '/client-home', name: "ClientHome", component: HomePageUserClient },
        { path: '/entrepeneur-home', name: "entrepeneurhome", component: HomePageEntrepeneur },

        { path: '/register', name: "Register", component: RegisterForm },
        { path: '/login', name: "Login", component: LoginForm },
        { path: '/email-verification', name: "VerifyEmail", component: EmailVerification },
        { path: '/profile', name: "Profile", component: ProfilePage },
        { path: '/register-address', name: "RegisterAddress", component: RegisterAddressForm },

        { path: '/enterprise-register', name: 'EnterpriseRegisterForm', component: EnterpriseRegisterForm },
        { path: '/enterprises', name: 'EntrepreneurRegisteredEnterprises', component: EntrepreneurRegisteredEnterprises },
        { path: '/collaborator', name: "CollaboratorProfile", component: CollaboratorProfilePage },
        { path: '/enterprise/:id', name: "EnterpriseDashboard", component: EnterpriseDashboard },
        { path: '/product', name: "ProductRegisterForm", component: ProductRegisterForm },

        { path: '/developer-products', name: "DeveloperProducts", component: DeveloperProducts },
        { path: '/developer-home', name: "DeveloperHome", component: HomePageDeveloper },
        { path: '/developer-users', name: "DeveloperUsers", component: DeveloperUsers },
        { path: '/developer-enterprises', name: "DeveloperEnterprises", component: DeveloperEnterprises },
    ]
});

router.beforeEach((to, from, next) => {
    const token = getToken();

    if (to.meta.requiresAuth && !token) {
        next('/login');
    } else {
        next();
    }
});

const app = createApp(App);
app.use(BootstrapVue3);
app.use(router);
app.config.globalProperties.$swal = Swal;
app.mount('#app');