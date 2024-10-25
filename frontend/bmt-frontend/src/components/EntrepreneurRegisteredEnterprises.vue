<template>
  <v-app class="d-flex flex-column">
    <!-- HEADER -->
    <v-app-bar :elevation="10" app color="#9FC9FC" scroll-behavior="hide" dark>
      <v-btn icon @click="menuDrawer = !menuDrawer">
        <v-icon>mdi-menu</v-icon>
      </v-btn>
      <v-toolbar-title>Business Tracker</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn color="primary" @click="goBack">Volver</v-btn>
    </v-app-bar>

    <!-- SIDEBAR -->
    <v-navigation-drawer v-model="menuDrawer" app color="#39517B">
      <v-list dense>
        <v-list-item @click="handleProfileInfo">
            <v-list-item-title>Mi perfil</v-list-item-title>
          </v-list-item>
          <v-list-item @click="handleEntrepreneurEnterprises">
            <v-list-item-title>Mis emprendimientos</v-list-item-title>
          </v-list-item>
          <v-list-item @click="handleCollaboratorRegister">
            <v-list-item-title>Registrar Colaborador</v-list-item-title>
          </v-list-item>
          <v-list-item @click="handleRegisterEnterprise">
            <v-list-item-title>Registrar Emprendimiento</v-list-item-title>
          </v-list-item>
          <v-list-item @click="handleProductRegister">
            <v-list-item-title>Registrar Producto</v-list-item-title>
          </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <!-- MAIN CONTENT -->
    <v-main class="flex-grow-1">
      <v-container>
            <h2 class="text-center">Emprendimientos asociados</h2>
            <v-card class="mb-4">
              <v-data-table height="600px" fixed header>
                <thead>
                  <tr>
                    <th>Nombre</th>
                    <th>Cédula</th>
                    <th>Administrador</th>
                    <th>Correo</th>
                    <th>Número de teléfono</th>
                    <th>Descripción</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="enterprise in enterprises" :key="enterprise.identificationNumber" @click="goToEnterprise(enterprise.id)">
                    <td>{{ enterprise.enterpriseName }}</td>
                    <td>{{ formatIdentification(enterprise.identificationNumber) }}</td>
                    <td>{{ enterprise.adminName }} {{ enterprise.adminLastName }}</td>
                    <td>{{ enterprise.email }}</td>
                    <td>{{ enterprise.phoneNumber }}</td>
                    <td>{{ enterprise.description }}</td>
                  </tr>
                </tbody>
              </v-data-table>
            </v-card>
      </v-container>
    </v-main>

    <!-- FOOTER -->
    <v-footer app padless color="#9FC9FC" dark>
      <v-col class="text-center white--text">
        &copy; 2024 Business Tracker. Todos los derechos reservados.
      </v-col>
    </v-footer>
  </v-app>
</template>

<script>
import axios from "axios";
import { getToken } from '@/helpers/auth';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      menuDrawer: false,
      enterprises: [],
      entrepreneur: {
        Id: '',
        Username: '',
        Identification: '',
      }
    };
  },
  mounted() {
    this.GetEnterprisesOfEntrepreneur();
  },
  methods: {
    async GetEnterprisesOfEntrepreneur() {
      try {
        const token = getToken();
        const user = JSON.parse(localStorage.getItem('user'));

        if (!user || !user.id || !user.name || !user.lastName || !user.username || !user.email || !user.password || user.isVerified === undefined) {
          console.error('Faltan datos del usuario');
          return;
        }
        const obtainEntrepreneurResponse = await axios.post(API_URL + '/Entrepreneur/GetEntrepreneurByUserId?id=' + user.id,
          {
            headers: {
              Authorization: `Bearer ${token}`
            }
          }
        );
        const entrepreneur = obtainEntrepreneurResponse.data;
        console.log(entrepreneur);
        const enterprisesResponse = await axios.post(
          API_URL + '/Entrepreneur/my-registered-enterprises?Identification=' + entrepreneur.identification,
          {
            headers: {
              Authorization: `Bearer ${token}`
            }
          });
        this.enterprises = enterprisesResponse.data;
        console.log(this.enterprises);

      } catch (error) {
        console.error('Error al obtener las empresas:', error);
        if (error.response) {
          console.error('Datos de la respuesta del servidor:', error.response.data);
        }
      }
    },
    formatIdentification(identification) {
      if (identification.length === 9) {
        return `${identification.slice(0, 1)}-${identification.slice(1, 5)}-${identification.slice(5)}`;
      }
      return identification;
    },
    goBack() {
      window.location.href = "/";
    },

    goToEnterprise(enterpriseId) {
      if (!enterpriseId) {
        console.error('El ID de la empresa es undefined');
        return;
      }
      console.log(enterpriseId);
      this.$router.push(`/enterprise/${enterpriseId}`);
    }
  }
};
</script>
<style scoped>
.v-app-bar {
  background-color: #9FC9FC;
}

.v-footer {
  height: 50px;
  background-color: #9FC9FC;
}

.text-center {
  text-align: center;
}

.v-card {
  background-color: #A9C5FF;
}

.v-data-table th {
  background-color: #39517B;
  color: white;
}

.v-data-table tbody tr {
  background-color: #FFF;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.v-data-table tbody tr:hover {
  background-color: #e0e0e0; 
}

.v-main {
  background-color: #FFF;
}

.v-navigation-drawer {
  background-color: #39517B;
}
</style>

