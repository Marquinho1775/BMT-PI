<template>
  <div class="profile-container d-flex justify-content-center align-items-center vh-100">
    <div class="card profile-card" style="max-width: 900px; width: 100%;">
      <!-- Header with Profile Picture and Name -->
      <div class="card-header profile-card-header d-flex align-items-center">
        <b-avatar src="https://placekitten.com/300/300" size="7rem" class="mr-3"></b-avatar>
        <div class="profile-name-container">
          <h2 class="mb-0">{{ user.name }} {{ user.lastName }}</h2>
        </div>
      </div>

      <!-- Profile Information Section -->
      <div class="card-body">
        <div class="row mb-3">
          <div class="col-md-6">
            <h4>Nombre</h4>
            <p>{{ user.name }} {{ user.lastName }}</p>
          </div>
          <div class="col-md-6">
            <h4>Email</h4>
            <p>{{ user.email }}</p>
          </div>
        </div>
        <div class="row mb-3">
          <div class="col-md-6">
            <h4>Username</h4>
            <p>{{ user.username }}</p>
          </div>
          <div class="col-md-6">
            <h4>Contraseña</h4>
            <p>
              <span v-if="showPassword">{{ user.password }}</span>
              <span v-else>••••••••</span>
              <button @click="togglePasswordVisibility" class="btn btn-link p-0 ml-2">
                {{ showPassword ? 'Ocultar' : 'Ver' }}
              </button>
            </p>
          </div>
          <div class="d-flex justify-content-between">
            <b-button variant="secondary" @click="goBack">Volver</b-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>



<script>
export default {
  data() {
    return {
      user: {
        name: '',
        lastName: '',
        username: '',
        email: '',
        password: ''
      },
      showPassword: false
    };
  },
  created() {
    if (!localStorage.getItem('token')) {
      window.location.href = "/login";
    } else {
      this.user = JSON.parse(localStorage.getItem('user')) || this.user;
    }
  },
  methods: {
    goBack() {
      window.history.back();
    },
    togglePasswordVisibility() {
      this.showPassword = !this.showPassword;
    }
  }
};
</script>


<style scoped>
.col-md-6 {
  border-bottom: #FFFFFF;
}

.btn-link {
  color: #007BFF;
  text-decoration: none;
  cursor: pointer;
  margin-left: 20px;
}

.profile-name-container {
  margin-left: 20px;
}

.profile-container {
  background-color: #D1E4FF;
}

.profile-card-header {
  background-color: #36618E;
  color: white;
  padding: 20px;
  border-radius: 20px 20px 0 0;
}

.profile-card {
  background-color: #9FC9FC;
  border-radius: 20px;
  margin: 0;
}
</style>
