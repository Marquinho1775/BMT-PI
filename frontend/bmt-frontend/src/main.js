import { createApp } from 'vue';
import App from './App.vue';
import { createRouter, createWebHistory } from 'vue-router';
import HomePage from './components/HomePage.vue';
import RegisterForm from './components/RegisterForm.vue';
import LoginForm from './components/LoginForm.vue';
import EnterpriseRegisterForm from './components/EnterpriseRegisterForm.vue';

// Import Bootstrap and BootstrapVue
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue-3/dist/bootstrap-vue-3.css';

// Import BootstrapVue3
import BootstrapVue3 from 'bootstrap-vue-3';

// Import SweetAlert2
import Swal from 'sweetalert2';


const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', name: "Home", component: HomePage },
        { path: '/register', name: "Register", component: RegisterForm },
        { path: '/login', name: "Login", component: LoginForm },
        { path: '/enterprise-register', name: "EnterpriseRegister", component: EnterpriseRegisterForm },
    ]
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
