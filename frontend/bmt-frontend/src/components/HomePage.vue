<template>
  <v-main class="flex-grow-1">
    <UserDashboard />
  </v-main>
</template>
  
<script>
import UserDashboard from './UserDashboard.vue';

  export default {
  components: {
    UserDashboard
      },
  data() {
    return {
      userRole: '',
    }
      },
  created() {
    if (!localStorage.getItem("token")) {
      this.$router.push('/login');
      }
    const user = JSON.parse(localStorage.getItem('user'));
    this.userRole = user.role;
    if (this.userRole === 'dev') {
      this.$router.push('/developer-dashboard');
    }
  }
};

</script>
  
<style scoped>
.flex-grow-1 {
  flex-grow: 1;
  }
</style>
  