<template>
    <v-main>
        <h1>Mis reportes</h1>

        <orders-reports :Type="1" />
        <orders-reports :Type="2" />
        <orders-reports :Type="3" />

        <yearly-earnings v-if="userRole === 'dev'" />
    </v-main>
</template>

<script>

export default {
    data() {
        return {
            userRole: "",
        };
    },
    created() {
        this.userRole = JSON.parse(localStorage.getItem("user")).role;
    },
}
</script>

<style lang="scss" scoped></style>