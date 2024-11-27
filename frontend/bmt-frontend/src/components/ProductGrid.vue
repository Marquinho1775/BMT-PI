<template>
  <v-card>
    <v-container class="pa-2" fluid>
      <v-row dense>
        <v-col v-for="item in products.filter(p => !p.isSoftDeleted)" :key="item.id" cols="auto" md="4">
          <product-card :product="item" />
        </v-col>
      </v-row>
    </v-container>
  </v-card>
</template>

<script>
export default {
  props: {
    products: {
      type: Array,
      required: true,
    },
  },
  data: () => ({
    search: '',
  }),
}
</script>
