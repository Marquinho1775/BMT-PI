<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
    <v-main class="flex-grow-1">
      <v-container>
        <v-card class="pa-5 mb-4">
          <v-row>
            <v-col>
              <h2 class="text-center">{{ enterprise.name }}</h2>
            </v-col>
          </v-row>
          <v-card-text>
            <p><strong>Cédula del emprendimiento:</strong> {{ enterprise.identificationNumber ? formatIdentification(enterprise.identificationNumber) : 'N/A' }}</p>
            <p><strong>Correo empresarial:</strong> {{ enterprise.email || 'N/A' }}</p>
            <p><strong>Número de teléfono:</strong> {{ enterprise.phoneNumber || 'N/A' }}</p>
            <h3>Emprendedores asociados</h3>
            <v-btn append-icon="mdi-plus" variant="outlined" @click="handleInviteEntrepreneur">
              Invitar colaborador 
            </v-btn>
            <div v-if="enterprise.staff && enterprise.staff.length">
              <ul>
                <li v-for="staff in enterprise.staff" :key="staff.id">
                  {{ staff.name }} {{ staff.lastName }}
                </li>
              </ul>
            </div>
          </v-card-text>
          <v-card-title>
            <h3 class="text-left">Productos</h3>
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col cols="6">
                <v-btn append-icon="mdi-plus" variant="outlined" @click="handleRegisterProduct">
                  Registrar nuevo producto
                </v-btn>
              </v-col>
              <v-col cols="6" class="d-flex justify-end">
                <v-btn variant="outlined" @click="handleInventoryView">
                  Ver Inventario
                </v-btn>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </v-container>
    </v-main>
    <AppFooter />
    <AppSidebar />
  </v-app>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';
import { getToken } from '@/helpers/auth';

export default {
  data() {
    return {
      enterprise: {}
    };
  },
  async created() {
    const enterpriseId = this.$route.params.id;
    try {
      const token = getToken();
      const enterpriseResponse = await axios.get(API_URL + `/Enterprise/${enterpriseId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      this.enterprise = enterpriseResponse.data;
    } catch (error) {
      console.error('Error al cargar la empresa:', error);
      if (error.response) {
        console.error('Detalles del error:', error.response.data);
      }
    }
  },
  methods: {
    formatIdentification(identification) {
      if (identification && identification.length === 9) {
        return `${identification.slice(0, 1)}-${identification.slice(1, 5)}-${identification.slice(5)}`;
      }
      return identification || 'N/A';
    },
    handleInviteEntrepreneur() {
      this.$router.push(`/enterprise/${this.enterprise.id}/invite`);
    },
    handleRegisterProduct() {
      this.$router.push(`${this.enterprise.id}/new-product`);
    },
    handleInventoryView() {
      this.$router.push(`${this.enterprise.id}/inventory`);
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

.v-card {
  background-color: #A9C5FF;
}

.text-center {
  text-align: center;
}

ul {
  padding-left: 20px;
  list-style-type: disc;
  margin-top: 10px;
}
</style>
