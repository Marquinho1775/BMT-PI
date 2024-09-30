<template>
    <div>

    </div>
</template>



<script>
import axios from "axios";
export default {

    data() {
        return {
            enterprises: [],
            users: [],
        }
    },

    methods: {
        async getEnterprisesOfEntrepreneur(){
            try {
                const response = await axios.post('https://localhost:7189/api/Entrepreneur/my-registered-enterprises', {
                    
                });
                this.enterprises = response.data;
            } catch (error) {
                console.error(error);
            }
        },
        },

    setup () {
        

        return {}
    }
}
</script>

<style lang="scss" scoped>

</style>