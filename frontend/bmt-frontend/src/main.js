import { createApp } from 'vue';
import App from './App.vue';
import { createRouter, createWebHistory } from 'vue-router';
import HomePage from './components/HomePage.vue';
import HomePageUserClient from './components/HomePageUserClient.vue';
import HomePageEntrepeneur from './components/HomePageEntrepeneur.vue';
import RegisterForm from './components/RegisterForm.vue';
import LoginForm from './components/LoginForm.vue';
import EnterpriseRegisterForm from './components/EnterpriseRegisterForm.vue';
import CollaboratorRegisterForm from './components/CollaboratorRegisterForm.vue';

import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue-3/dist/bootstrap-vue-3.css';
import BootstrapVue3 from 'bootstrap-vue-3';

import Swal from 'sweetalert2';

import { getToken } from './helpers/auth';
import axios from 'axios';

// Inicialización de interceptor axios
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
        { path: '/clienthome', name: "ClientHome", component: HomePageUserClient},
        { path: '/entrepeneurhome', name: "Entrepeneurhome", component: HomePageEntrepeneur },
        { path: '/register', name: "Register", component: RegisterForm },
        { path: '/login', name: "Login", component: LoginForm },
        { path: '/enterpriseregister', name: "EnterpriseRegister", component: EnterpriseRegisterForm },
        { path: '/collabregister', name: "CollabRegister", component: CollaboratorRegisterForm },
    ]
});

// Añadir ruta guardián a las que requieran autenticación
router.beforeEach((to, from, next) => {
    const token = getToken();

    // Redirigir al login si no hay token y la ruta requiere autenticación
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
