import { createApp } from 'vue';
import App from './App.vue';
import VueGoogleMaps from '@fawmi/vue-google-maps';
import { createRouter, createWebHistory } from 'vue-router';

// Importar todos tus componentes aquí
import HomePage from './components/HomePage.vue';
import UserRegisterForm from './components/UserRegisterForm.vue';
import UserLoginForm from './components/UserLoginForm.vue';
import UserEmailVerification from './components/UserEmailVerification.vue';
import UserProfilePage from './components/UserProfilePage.vue';
import UserRegisterAddress from './components/UserRegisterAddress.vue';
import UserShoppingCart from './components/UserShoppingCart.vue';
import UserCardForm from './components/UserCardForm.vue';
import UserOrders from './components/UserOrders.vue';
import UserDelete from './components/UserDeleteConfirmation.vue';
import EditProfileInfo from './components/EditProfileInfo.vue';
import EditEnterpriseInfo from './components/EditEnterpriseInfo.vue';
import RedirectionerPage from './components/RedirectionerPage.vue';
import EntrepreneurRegisteredEnterprises from './components/EntrepreneurRegisteredEnterprises.vue';
import EnterpriseRegisterForm from './components/EnterpriseRegisterForm.vue';
import EnterpriseDashboard from './components/EnterpriseDashboard.vue';
import EnterpriseInventory from './components/EnterpriseInventory.vue';
import CollaboratorRegisterForm from './components/CollaboratorRegisterForm.vue';
import AcceptInvitation from './components/AcceptInvitation.vue';
import ProductRegisterForm from './components/ProductRegisterForm.vue';
import DeveloperDashboard from './components/DeveloperDashboard.vue';
import DeveloperEnterprises from './components/DeveloperEnterprises.vue';
import DeveloperProducts from './components/DeveloperProducts.vue';
import DeveloperUsers from './components/DeveloperUsers.vue';
import DeveloperOrderConfirmation from './components/DeveloperOrderConfirmation.vue';
import CheckOut from './components/CheckOut.vue';
import ProductCard from './components/ProductCard.vue';
import ProductGrid from './components/ProductGrid.vue';
import AppHeader from './components/AppHeader.vue';
import AppSidebar from './components/AppSidebar.vue';
import AppFooter from './components/AppFooter.vue';
import UserDashboard from './components/UserDashboard.vue';
import SearchResultPage from './components/SearchResultPage.vue';
import OrdersReports from './components/OrdersReports.vue';
import ReportsTable from './components/ReportsTable.vue';
import YearlyEarnings from './components/YearlyEarnings.vue';
import ReportsDashboard from './components/ReportsDashboard.vue';
import StackedBarChart from './components/StackedBarChart.vue';
import LineChart from './components/LineChart.vue';
import PendingOrders from './components/PendingOrders.vue';
import ChatGPT from './components/ChatGPT.vue';

import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue-3/dist/bootstrap-vue-3.css';
import BootstrapVue3 from 'bootstrap-vue-3';
import Swal from 'sweetalert2';

import vuetify from './plugins/vuetify';
import { loadFonts } from './plugins/webfontloader';

// Importar las utilidades de autenticación
import { getToken } from './helpers/auth';
import axios from 'axios';

export const URL = 'https://localhost:7189/';
export const API_URL = URL + 'api';

// Configurar interceptores de Axios
axios.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: "RedirectionerPage", component: RedirectionerPage },
    { path: '/home', name: "Home", component: HomePage },
    { path: '/register', name: "Register", component: UserRegisterForm },
    { path: '/login', name: "Login", component: UserLoginForm },
    { path: '/email-verification', name: "VerifyEmail", component: UserEmailVerification },
    { path: '/profile', name: "Profile", component: UserProfilePage },
    { path: '/register-address', name: "RegisterAddress", component: UserRegisterAddress },
    { path: '/card-form', name: "CardForm", component: UserCardForm },
    { path: '/shopping-cart', name: "ShoppingCart", component: UserShoppingCart },
    { path: '/enterprise-register', name: 'EnterpriseRegisterForm', component: EnterpriseRegisterForm },
    { path: '/enterprises', name: 'EntrepreneurRegisteredEnterprises', component: EntrepreneurRegisteredEnterprises },
    { path: '/enterprise/:id', name: "EnterpriseDashboard", component: EnterpriseDashboard },
    { path: '/enterprise/:id/inventory', name: "EnterpriseInventory", component: EnterpriseInventory },
    { path: '/enterprise/:id/edit', name: "EditEnterpriseInfo", component: EditEnterpriseInfo },
    { path: '/enterprise/:id/invite', name: "CollaboratorRegisterForm", component: CollaboratorRegisterForm },
    { path: '/acceptInvite', name: "AcceptInvitation", component: AcceptInvitation },
    { path: '/enterprise/:id/new-product', name: "ProductRegisterForm", component: ProductRegisterForm },
    { path: '/checkout', name: "CheckOut", component: CheckOut },
    { path: '/orders', name: "Orders", component: UserOrders },
    { path: '/search/:searchText', name: "SearchResultPage", component: SearchResultPage },
    { path: '/reports', name: "ReportsDashboard", component: ReportsDashboard },
    { path: '/developer-dashboard', name: "DeveloperDashboard", component: DeveloperDashboard },
    { path: '/developer-products', name: "DeveloperProducts", component: DeveloperProducts },
    { path: '/developer-users', name: "DeveloperUsers", component: DeveloperUsers },
    { path: '/developer-enterprises', name: "DeveloperEnterprises", component: DeveloperEnterprises },
    { path: '/products-confirmation', name: "ProductsConfirmation", component: DeveloperOrderConfirmation },
    { path: '/profile/edit', name: "EditProfile", component: EditProfileInfo },
    { path: '/UserDeleteConfirmation', name: "DeleteUserConfirmation", component: UserDelete },
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
app.use(vuetify);
app.use(router);
app.use(VueGoogleMaps, {
  load: {
    key: process.env.VUE_APP_GOOGLE_MAPS_API_KEY,
  },
});

// Registrar componentes globales
app.component('product-card', ProductCard);
app.component('productGrid', ProductGrid);
app.component('AppHeader', AppHeader);
app.component('AppSidebar', AppSidebar);
app.component('AppFooter', AppFooter);
app.component('reports-table', ReportsTable);
app.component('orders-reports', OrdersReports);
app.component('yearly-earnings', YearlyEarnings);
app.component('StackedBarChart', StackedBarChart);
app.component('LineChart', LineChart);
app.component('UserDashboard', UserDashboard);
app.component('pending-orders', PendingOrders);
app.component('ChatGPT', ChatGPT); // Registrar globalmente

app.config.globalProperties.$swal = Swal;

app.mount('#app');
