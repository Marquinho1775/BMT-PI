<template>
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
            <tr v-for="enterprise in enterprises" :key="enterprise.identificationNumber"
              @click="goToEnterprise(enterprise.id)">
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
        const enterprisesResponse = await axios.post(
          API_URL + '/Entrepreneur/my-registered-enterprises?Identification=' + entrepreneur.identification,
          {
            headers: {
              Authorization: `Bearer ${token}`
            }
          });
        this.enterprises = enterprisesResponse.data;

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
