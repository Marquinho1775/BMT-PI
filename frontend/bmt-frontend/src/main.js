
import { createApp } from 'vue';
import App from './App.vue';
import VueGoogleMaps from '@fawmi/vue-google-maps';
import { createRouter, createWebHistory } from 'vue-router';
import HomePage from './components/HomePage.vue';

import UserRegisterForm from './components/UserRegisterForm.vue';
import UserLoginForm from './components/UserLoginForm.vue';
import UserEmailVerification from './components/UserEmailVerification.vue';
import UserProfilePage from './components/UserProfilePage.vue';
import UserRegisterAddress from './components/UserRegisterAddress.vue';
import UserShoppingCart from './components/UserShoppingCart.vue';

import EditProfileInfo from './components/EditProfileInfo.vue';
import EditEnterpriseInfo from './components/EditEnterpriseInfo.vue';

import EntrepreneurRegisteredEnterprises from './components/EntrepreneurRegisteredEnterprises.vue';
import EntrepreneurProfilePage from './components/EntrepreneurProfilePage.vue';

import EnterpriseRegisterForm from './components/EnterpriseRegisterForm.vue';
import EnterpriseDashboard from './components/EnterpriseDashboard.vue';
import EnterpriseInventory from './components/EnterpriseInventory.vue';
import CollaboratorRegisterForm from './components/CollaboratorRegisterForm.vue';
import AcceptInvitation from './components/AcceptInvitation.vue';

import ProductRegisterForm from './components/ProductRegisterForm.vue';

import DeveloperEnterprises from './components/DeveloperEnterprises.vue';
import DeveloperProducts from './components/DeveloperProducts.vue';
import DeveloperUsers from './components/DeveloperUsers.vue';

import ProductCard from './components/ProductCard.vue';
import ProductSearchGrid from './components/ProductSearchGrid.vue';
import AppHeader from './components/AppHeader.vue';
import AppSidebar from './components/AppSidebar.vue';
import AppFooter from './components/AppFooter.vue';

import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue-3/dist/bootstrap-vue-3.css';
import BootstrapVue3 from 'bootstrap-vue-3';
import Swal from 'sweetalert2';

import vuetify from './plugins/vuetify'
import { loadFonts } from './plugins/webfontloader'


// Import the authentication utilities
import { getToken } from './helpers/auth';
import axios from 'axios';

export const URL = 'https://localhost:7189/';
export const API_URL = URL + 'api';

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

    { path: '/register', name: "Register", component: UserRegisterForm },
    { path: '/login', name: "Login", component: UserLoginForm },
    { path: '/email-verification', name: "VerifyEmail", component: UserEmailVerification },
    { path: '/profile', name: "Profile", component: UserProfilePage },
    { path: '/register-address', name: "RegisterAddress", component: UserRegisterAddress },
    {path: '/shopping-cart', name: "ShoppingCart", component: UserShoppingCart},

    { path: '/enterprise-register', name: 'EnterpriseRegisterForm', component: EnterpriseRegisterForm },
    { path: '/enterprise/:id', name: "EnterpriseDashboard", component: EnterpriseDashboard },
    { path: '/enterprise/:id/inventory', name: "EnterpriseInventory", component: EnterpriseInventory },
    { path: '/enterprise/:id/edit', name: "EditEnterpriseInfo", component: EditEnterpriseInfo },
    { path: '/enterprise/:id/invite', name: "CollaboratorRegisterForm", component: CollaboratorRegisterForm },
    { path: '/acceptInvite', name: "AcceptInvitation", component: AcceptInvitation },

    { path: '/enterprises', name: 'EntrepreneurRegisteredEnterprises', component: EntrepreneurRegisteredEnterprises },
    { path: '/entrepreneur', name: "EntrepreneurProfile", component: EntrepreneurProfilePage },

    { path: '/enterprise/:id/new-product', name: "ProductRegisterForm", component: ProductRegisterForm },

    { path: '/developer-products', name: "DeveloperProducts", component: DeveloperProducts },
    { path: '/developer-users', name: "DeveloperUsers", component: DeveloperUsers },
    { path: '/developer-enterprises', name: "DeveloperEnterprises", component: DeveloperEnterprises },
    { path: '/profile/edit', name: "EditProfile", component: EditProfileInfo },
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

loadFonts();

const app = createApp(App);

app.use(BootstrapVue3);
app.use(vuetify)
app.use(router);
app.use(VueGoogleMaps, {
  load: {
    key: process.env.VUE_APP_GOOGLE_MAPS_API_KEY, // Load the API key from the .env file
  },
});

app.component('product-card', ProductCard);
app.component('productSearchGrid', ProductSearchGrid);
app.component('AppHeader', AppHeader);
app.component('AppSidebar', AppSidebar);
app.component('AppFooter', AppFooter);

app.config.globalProperties.$swal = Swal;

app.mount('#app');