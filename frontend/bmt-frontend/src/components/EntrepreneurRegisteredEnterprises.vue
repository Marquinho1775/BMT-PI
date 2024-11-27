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
              <th>Acciones</th> <!-- Nueva columna -->
              </tr>
            </thead>
            <tbody>
            <tr 
              v-for="enterprise in enterprises" 
              :key="enterprise.identificationNumber"
              @click="goToEnterprise(enterprise.id)"
            >
              <td>{{ enterprise.name }}</td>
                <td>{{ formatIdentification(enterprise.identificationNumber) }}</td>
              <td>{{ enterprise.administrator.name }} {{ enterprise.administrator.lastname }}</td>
              <td>{{ enterprise.email }}</td>
              <td>{{ enterprise.phoneNumber }}</td>
                <td>{{ enterprise.description }}</td>
              <td>
                <v-icon
                  color="red"
                  size="18"
                  @click.stop="confirmDeleteEnterprise(enterprise)"
                >
                  mdi-delete
                </v-icon>
              </td>
              </tr>
            </tbody>
        </v-data-table>
      </v-card>
    </v-container>
  </v-main>
</template>

<script>
import axios from "axios";
import { getToken } from "@/helpers/auth";
import { API_URL } from "@/main.js";

export default {
  data() {
    return {
      menuDrawer: false,
      enterprises: [],
      entrepreneur: {
        Id: "",
        Username: "",
        Identification: "",
      },
    };
  },
  mounted() {
    this.GetEnterprisesOfEntrepreneur();
  },
  methods: {
    async GetEnterprisesOfEntrepreneur() {
      try {
        const token = getToken();
        const user = JSON.parse(localStorage.getItem("user"));
        const obtainEntrepreneurResponse = await axios.get(
          API_URL + "/Entrepreneur/GetEntrepreneurByUserId?id=" + user.id,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        const entrepreneurId = obtainEntrepreneurResponse.data.identification;
        const enterprisesResponse = await axios.get(
          API_URL +
            "/Entrepreneur/my-registered-enterprises?Identification=" +
            entrepreneurId,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        this.enterprises = enterprisesResponse.data.success;
      } catch (error) {
        console.error("Error al obtener las empresas:", error);
        if (error.response) {
          console.error("Datos de la respuesta del servidor:", error.response.data);
        }
      }
    },
    formatIdentification(identification) {
      if (identification.length === 9) {
        return `${identification.slice(0, 1)}-${identification.slice(1, 5)}-${identification.slice(5)}`;
      }
      return identification;
    },
    async confirmDeleteEnterprise(enterprise) {
      const result = await this.$swal.fire({
        title: "¿Estás seguro?",
        text: `Vas a borrar el emprendimiento: ${enterprise.name}`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sí, borrar",
        cancelButtonText: "Cancelar",
      });

      if (result.isConfirmed) {
        this.deleteEnterprise(enterprise);
      }
    },
    async deleteEnterprise(enterprise) {
      try {
        const token = getToken();
        const response = await axios.delete(
          `${API_URL}/Enterprise/Delete/${enterprise.id}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.status === 200) {
          this.enterprises = this.enterprises.filter((e) => e.id !== enterprise.id);

          this.$swal.fire({
            title: "¡Borrado!",
            text: `El emprendimiento ${enterprise.name} ha sido eliminado.`,
            icon: "success",
          });
        } else {
          throw new Error("Error inesperado al borrar el emprendimiento.");
        }
      } catch (error) {
        console.error(error);
        this.$swal.fire({
          title: "Error",
          text: "No se pudo borrar el emprendimiento. Inténtalo nuevamente.",
          icon: "error",
        });
      }
    },
    goBack() {
      window.location.href = "/";
    },

    goToEnterprise(enterpriseId) {
      if (!enterpriseId) {
        console.error("El ID de la empresa es undefined");
        return;
      }
      console.log(enterpriseId); // Asegúrate de que el ID no sea undefined
      this.$router.push(`/enterprise/${enterpriseId}`);
    },
  },
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

.v-data-table th {
  background-color: #39517B;
  font-weight: bold;
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
