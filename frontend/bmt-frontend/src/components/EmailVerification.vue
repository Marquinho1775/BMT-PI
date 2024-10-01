<template>
  <div class="d-flex justify-content-center align-items-center full-height background-main">
    <div class="card custom-card">
      <h3 class="text-center card-header-custom">Verifica tu correo</h3>
      <div class="card-body">
        <p class="text-center text-custom">Por favor introduce el código de verificación que se envió al correo electrónico que proporcionaste</p>
        <div class="form-group">
          <label for="verificationCode" class="label-custom">Código de verificación</label>
          <input type="text" class="form-control custom-placeholder" id="verificationCode" v-model="verificationCode" placeholder="Inserte el código">
        </div>
        <div class="d-grid gap-2">
          <button class="btn btn-custom btn-block mt-3" :disabled="isLoading" @click="verifyCode">
            {{ isLoading ? 'Verificando...' : 'Verificar Correo' }}
          </button>
        </div>
        <p class="text-center mt-3">
          <a href="#" @click="resendCode" class="resend-link">¿No recibiste el código? Click para reenviar</a>
        </p>  
        <div v-if="message" class="alert alert-danger mt-3" role="alert">
          {{ message }}
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { getUser } from '@/helpers/auth';
import axios from 'axios';

export default {
  data() {
    return {
      verificationCode: '',
      message: '',
      isLoading: false
    };
  },
  mounted() {
    this.resendCode();
  },
  methods: {
    async verifyCode() {
      this.isLoading = true;
      this.message = '';
      try {
        const codeTaken = {
          Code: this.verificationCode,
          Id: getUser().id
        };
        await axios.post('https://localhost:7189/api/Email/verifycode', codeTaken)
          .then(() => {
            this.$swal.fire({
              title: 'Verificación exitosa',
              text: '¡La cuenta ha sido verificada correctamente!',
              icon: 'success',
              confirmButtonText: 'Ok'
            })
          }).then(response => {
            console.log('Verificación exitosa:', response);
            axios.post('https://localhost:7189/api/Email/verifyaccount', codeTaken)
            .then(() => {
              console.log('Cuenta verificada:', response);
              window.location.href = "/";
            })
          }).catch(error => {
            console.error('Error en la verificación:', error);
          });
      } catch (error) {
        this.message = 'Error en la verificación. Inténtalo de nuevo más tarde.';
      } finally {         
        this.isLoading = false;
      }
    },
    async resendCode() {
      try {
        const correo = {
          Email: getUser().email,
          Id: getUser().id
        };
        axios.post('https:/localhost:7189/api/Email/sendemail', correo)
        .then(response => {
            console.log('Reenvío con exito:', response);
            alert('Se ha enviado un correo con el código de verificación.');
        })
        .catch(error => {
            console.error('Error en la verificación:', error);
        });
      } catch (error) {
        this.message = 'Error en el envío del código.';
      } finally {
        this.isLoading = false;
      }
    }
  }
};
</script>

<style scoped>
.full-height {
  height: 100vh;
}

.background-main {
  background-color: #D1E4FF;
}

.custom-card {
  width: 650px;
  background-color: #9FC9FC;
  border-radius: 20px;
  margin: 0px;
}

.card-header-custom {
  background-color: #36618E;
  color: white;
  padding: 20px;
  border-radius: 20px 20px 0 0;
  width: 100%;
  height: 100%;
}

.text-custom {
  font-weight: 550;
  color: #49454F;
}

.label-custom {
  color: #49454F;
  font-weight: 500;
  padding: 5px;
}

.custom-placeholder {
  background-color: #D0EDA0;
}

.btn-custom {
  background-color: #36618E;
  color: white;
  border-radius: 50px;
  font-weight: 600;
  font-size: medium;
}

.btn-custom:hover {
  background-color: #447cb9;
  color: white;
}

.resend-link {
  color: #36618E;
  text-decoration: none;
  font-weight: 550; 
}

.resend-link:hover {
  text-decoration: underline;
}

.custom-placeholder::placeholder {
  color: #49454F;
  font-weight: 600;
  opacity: 0.5;
}

.custom-placeholder:focus {
  background-color: #D0EDA0;
}
</style>
