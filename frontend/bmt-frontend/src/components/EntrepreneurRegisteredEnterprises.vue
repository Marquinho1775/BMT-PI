<template>

  <body>
    <div class="entrepreneur-enterprises">
      <div class="content">
        <div class="container-fluid d-flex flex-column align-items-center justify-content-center" style="height: 10vh;">
          <div>
            <h2 class="title">Emprendimientos asociados</h2>
          </div>
        </div>
        <div class="table-container">
          <table class="enterprise-table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Cédula</th>
                <th>Administrador</th>
                <th>Descripción</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="enterprise in enterprises" :key="enterprise.identificationNumber"
                @click="goToEnterprise(enterprise.id)">
                <td>{{ enterprise.enterpriseName }}</td>
                <td>{{ formatIdentification(enterprise.identificationNumber) }}</td>
                <td>{{ enterprise.adminName }} {{ enterprise.adminLastName }}</td>
                <td>{{ enterprise.description }}</td>
              </tr>
            </tbody>
          </table>
          <div class="button-container">
            <button class="button" @click="goBack">Volver</button>
          </div>
        </div>
      </div>
    </div>
  </body>
</template>

<script>
import axios from "axios";
import { getToken } from '@/helpers/auth';

export default {
  data() {
    return {
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

        // Verificar si el usuario está presente y tiene todos los campos requeridos
        if (!user || !user.id || !user.name || !user.lastName || !user.username || !user.email || !user.password || user.isVerified === undefined) {
          console.error('Faltan datos del usuario');
          return;
        }
        const obtainEntrepreneurResponse = await axios.post(
          'https://localhost:7189/api/Entrepreneur/ObtainEntrepreneurBasedOnUser',
          {
            Id: user.id,
            Name: user.name,
            LastName: user.lastName,
            Username: user.username,
            Email: user.email,
            Password: user.password,
            IsVerified: user.isVerified
          },
          {
            headers: {
              Authorization: `Bearer ${token}`
            }
          }
        );
        const entrepreneur = obtainEntrepreneurResponse.data;
        const enterprisesResponse = await axios.post(
          'https://localhost:7189/api/Entrepreneur/my-registered-enterprises',
          entrepreneur,
          {
            headers: {
              Authorization: `Bearer ${token}`
            }
          }
        );
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
      this.$router.push('/entrepeneur-home');
    },

    goToEnterprise(enterpriseId) {
      if (!enterpriseId) {
        console.error('El ID de la empresa es undefined');
        return;
      }
      console.log(enterpriseId); // Asegúrate de que el ID no sea undefined
      this.$router.push(`/enterprise/${enterpriseId}`);
    }
  }
};
</script>

<style scoped>
.entrepreneur-enterprises {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #D1E4FF;
  justify-content: flex-start;
  align-items: center;
  padding-top: 20px;
}

.content {
  width: 80%;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

h2 {
  margin-bottom: 10px;
  margin-top: 0;
}

.table-container {
  background-color: #A9C5FF;
  padding: 20px;
  border-radius: 15px;
  width: 100%;
  min-height: 60vh;
  box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.enterprise-table {
  width: 100%;
  border-collapse: collapse;
  margin: 0 auto;
}

.enterprise-table th,
.enterprise-table td {
  padding: 10px;
  text-align: left;
}

.enterprise-table th {
  background-color: #39517B;
  font-weight: bold;
  color: white;
}

.enterprise-table td {
  background-color: #D0EDA0;
  border-bottom: 1px solid #ddd;
}

.title {
  background-color: #D0EDA0;
  color: #02174B;
  padding: 15px;
  border-radius: 100px;
  text-align: center;
}

.button-container {
  position: absolute;
  bottom: 20px;
  left: 20px;
}

.button {
  background-color: #39517B;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.button:hover {
  background-color: #2d3f5a;
}
</style>
