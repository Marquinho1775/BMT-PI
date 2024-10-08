<template>
    <div class="enterprise-dashboard">
      <div class="content">
        <div class="container-fluid d-flex flex-column align-items-center justify-content-center" style="height: 10vh;">
          <div>
            <h2 class="title">{{ enterprise.name}}</h2>
          </div>
        </div>
        <div class="details-container">
          <div class="enterprise-details">
            <p><strong>Cédula del emprendimiento:</strong> {{ enterprise.identificationNumber ? formatIdentification(enterprise.identificationNumber) : 'N/A' }}</p>
            <p><strong>Correo del administrador:</strong> {{ enterprise.administrator?.email || 'N/A' }}</p>
          </div>
  
          <div class="staff-list" v-if="enterprise.staff && enterprise.staff.length">
            <h3>Emprendedores asociados</h3>
            <ul>
              <li v-for="staff in enterprise.staff" :key="staff.id">{{ staff.name }} {{ staff.lastName }}</li>
            </ul>
          </div>
  
          <div class="product-list">
            <h3>Productos</h3>
            <p>Productos no disponibles por el momento</p>
          </div>
  
          <div class="button-container">
            <button class="button" @click="goBack">Volver</button>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script>
  import axios from 'axios';
  import { getToken } from '@/helpers/auth';
  
  export default {
    data() {
      return {
        enterprise: {
          administrator: {},
          staff: []
        }
      };
    },
    async created() {
      const enterpriseId = this.$route.params.id;
      try {
        const token = getToken();
        const enterpriseResponse = await axios.get(`https://localhost:7189/api/Enterprise/${enterpriseId}`, {
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
      goBack() {
        this.$router.push('/enterprises');
      }
    }
  };
  </script>
  
  <style scoped>
  .enterprise-dashboard {
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
  
  .title {
    background-color: #D0EDA0;
    color: #02174B;
    padding: 15px;
    border-radius: 100px;
    text-align: center;
  }
  
  .details-container {
    background-color: #A9C5FF;
    padding: 20px;
    border-radius: 15px;
    width: 100%;
    min-height: 60vh;
    box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
  }
  
  button {
    background-color: #39517B;
    color: white;
    padding: 10px 20px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  
  button:hover {
    background-color: #2d3f5a;
  }
  
  .staff-list, .product-list {
    margin-top: 20px;
  }
  
  .staff-list h3, .product-list h3 {
    margin-bottom: 10px;
  }
  </style>
  