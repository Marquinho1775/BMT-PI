<template>
  <div class="register-address-container d-flex justify-content-center align-items-center vh-100">
    <b-form @submit.prevent="registerAddress" @reset="onReset">
      <div id="form" class="card custom-card my-4">
        <h3 id="title" class="text-center card-header-custom">Registrar Dirección</h3>
        <div class="card-body">

          <b-form-group label="Nombre de la direccion" label-for="numDirection">
            <b-form-input id="Nombre de la direccion" v-model="addressData.numDirection" placeholder="Ingrese el nombre"
              required></b-form-input>
          </b-form-group>

          <b-form-group label="Provincia" label-for="province">
            <b-form-input id="province" v-model="addressData.province" placeholder="Ingrese la provincia"
              required></b-form-input>
          </b-form-group>

          <b-form-group label="Cantón" label-for="canton">
            <b-form-input id="canton" v-model="addressData.canton" placeholder="Ingrese el cantón"
              required></b-form-input>
          </b-form-group>

          <b-form-group label="Distrito" label-for="district">
            <b-form-input id="district" v-model="addressData.district" placeholder="Ingrese el distrito"
              required></b-form-input>
          </b-form-group>

          <b-form-group label="Otras señales" label-for="otherSigns">
            <b-form-textarea id="otherSigns" v-model="addressData.otherSigns" placeholder="Ingrese otras señales"
              rows="3"></b-form-textarea>
          </b-form-group>

          <b-form-group label="Coordenadas" label-for="coordinates">
            <b-form-input id="coordinates" v-model="addressData.coordinates" placeholder="Ingrese las coordenadas"
              required></b-form-input>
          </b-form-group>
        </div>
      </div>
      <div class="d-flex justify-content-between">
        <b-button variant="secondary" @click="goBack">Volver</b-button>
        <b-button type="submit" class="button">Registrar Dirección</b-button>
      </div>
    </b-form>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      addressData: {
        username: JSON.parse(localStorage.getItem('user')).username,
        numDirection: '',
        province: '',
        canton: '',
        district: '',
        otherSigns: '',
        coordinates: ''
      }
    };
  },
  methods: {
    async registerAddress() {
      try {
        const response = await axios.post('https://localhost:7189/api/Direction/CreateDirection', this.addressData);
        console.log(response);
        await this.$swal.fire({
          title: 'Registro exitoso',
          text: '¡Su dirección ha sido registrada correctamente!',
          icon: 'success',
          confirmButtonText: 'Ok'
        });

        // Después de la confirmación, redirigir al perfil
        this.$router.push('/profile');
      } catch (error) {
        console.error('Error al registrar la dirección:', error);
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al registrar su dirección. Inténtelo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
      }
    },
    onReset(event) {
      event.preventDefault();
      this.addressData = {
        username: JSON.parse(localStorage.getItem('user')).username,
        province: '',
        canton: '',
        district: '',
        otherSigns: '',
        coordinates: ''
      };
    },
    goBack() {
      this.$router.push('/profile');
    }
  }
};
</script>

<style scoped>
.register-address-container {
  background-color: #D1E4FF;
}

div.custom-card {
  max-width: 600px;
  background-color: #9FC9FC;
  border-radius: 20px;
  margin: 0px;
}

.card-header-custom {
  background-color: #36618E;
  color: white;
  padding: 20px;
  border-radius: 20px 20px 0 0;
}

.button {
  background-color: #39517B;
}

.button:hover {
  background-color: #02174B;
}
</style>