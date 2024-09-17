import { createApp } from 'vue';
import App from './App.vue';
import { createRouter, createWebHistory } from 'vue-router';
import HomePage from './components/HomePage.vue';
import RegisterForm from './components/RegisterForm.vue';
import LoginForm from './components/LoginForm.vue';


const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', name: "Home", component: HomePage },
        { path: '/register', name: "Register", component: RegisterForm },
        { path: '/login', name: "Login", component: LoginForm },
    ]
});

createApp(App).use(router).mount('#app')
