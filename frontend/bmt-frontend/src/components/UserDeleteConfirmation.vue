<template>
    <v-main class="flex-grow-1">
        <v-container class="d-flex justify-center align-center" style="min-height: 80vh;">
        <v-card class="custom-card pa-6">
          <!-- Header with Name, and Username -->
          <v-row class="align-center custom-header">
            <v-col class="d-flex flex-column">
              <h2 class="mb-1">{{ user.name }} {{ user.lastName }}</h2>
              <span class="text-body-2 grey--text">@{{ user.username }}</span>
              <span class="text-body-2 grey--text"> ¿En serio desea borrar su usuario y todo lo relacionado a este?</span>
              <span class="text-body-2 grey--text"> Esta operación es irreversible </span>
            </v-col>
          </v-row>
          
  
          <!-- Botón para eliminar cuenta -->
          <v-row class="mt-6 justify-center">
            <v-btn color="red" text @click="openDeleteDialog">
              <v-icon left>mdi-account-remove</v-icon> Eliminar Cuenta
            </v-btn>
          </v-row>
  
          <!-- Diálogo de confirmación de eliminación -->
          <v-dialog v-model="deleteDialog" max-width="500">
            <v-card>
              <v-card-title>Eliminación de Cuenta</v-card-title>
              <v-card-text>
                <p>¿Estás seguro de que deseas eliminar tu cuenta? Esta acción no se puede deshacer.</p>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="grey" text @click="cancelDelete">Cancelar</v-btn>
                <v-btn color="red" text @click="confirmDelete">Eliminar</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-card>
      </v-container>
    </v-main>
  </template>
  
  <script>
  import axios from "axios";
  import { API_URL } from "@/main.js";
  import { getToken } from "@/helpers/auth";
  
  export default {
    data() {
      return {
        deleteDialog: false,
        imageURL: "",
        user: {
          id: "",
          name: "",
          lastName: "",
          username: "",
          email: "",
        },
      };
    },
    created() {
      if (!localStorage.getItem("token")) {
        window.location.href = "/login";
      } else {
        this.user = JSON.parse(localStorage.getItem("user")) || this.user;
        this.imageURL = this.user.profilePictureURL || "";
      }
    },
    methods: {
      openDeleteDialog() {
        this.deleteDialog = true;
      },
      cancelDelete() {
        this.deleteDialog = false;
      },
      async confirmDelete() {
        try {
          const token = getToken();
          await axios.delete(`${API_URL}/User/Delete/${this.user.id}`, {
            headers: { Authorization: `Bearer ${token}` },
          });
  
          this.deleteDialog = false;
          // Muestra confirmación y redirige al usuario
          this.$swal.fire({
            title: "¡Cuenta eliminada!",
            text: "Tu cuenta ha sido eliminada exitosamente.",
            icon: "success",
            confirmButtonText: "Ok",
          }).then(() => {
            localStorage.clear(); // Limpia datos de sesión
            this.$router.push("/"); // Redirige a la página principal
          });
        } catch (error) {
          console.error("Error al eliminar la cuenta:", error);
          this.$swal.fire({
            title: "Error",
            text: "Hubo un problema al eliminar tu cuenta. Intenta nuevamente.",
            icon: "error",
            confirmButtonText: "Ok",
          });
        }
      },
    },
  };
  </script>
  
  <style scoped>
  .profile-card {
    background-color: #9fc9fc;
    border-radius: 20px;
  }
  
  .custom-card {
    border-left: 5px solid #4e88e6;
    background-color: #ffffff;
  }
  
  .text-body-2 {
    font-size: 14px;
    color: #6c757d;
  }
  
  .flex-grow-1 {
    flex-grow: 1;
  }
  
  .avatar-container {
    position: relative;
    cursor: pointer;
  }
  
  .profile-image {
    transition: 0.3s ease;
  }
  
  .overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    opacity: 0;
    transition: 0.3s ease;
    border-radius: 50%;
  }
  
  .edit-icon {
    color: white;
    font-size: 32px;
  }
  
  .v-card-title {
    display: flex;
    align-items: center;
  }
  
  .v-btn {
    text-transform: none;
  }
  
  .v-snackbar {
    max-width: 400px;
  }
  
  .credit-card {
    background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
    color: white;
    border-radius: 15px;
    position: relative;
    padding: 20px;
  }
  
  .card-chip {
    position: absolute;
    top: 20px;
    left: 20px;
  }
  
  .card-number {
    margin-top: 60px;
    font-size: 24px;
    letter-spacing: 3px;
    text-align: center;
  }
  
  .card-details {
    display: flex;
    justify-content: space-between;
    margin-top: 40px;
  }
  
  .card-holder,
  .card-expiry {
    font-size: 14px;
  }
  
  .card-holder span,
  .card-expiry span {
    font-size: 12px;
    opacity: 0.8;
  }
  
  .card-holder div,
  .card-expiry div {
    font-size: 16px;
  }
  </style>
  