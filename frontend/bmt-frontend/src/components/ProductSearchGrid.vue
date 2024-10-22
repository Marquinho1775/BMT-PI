<template>
    <v-card>
      <v-data-iterator :items="products" :items-per-page="25" :search="search">
        <template v-slot:header>
          <v-toolbar class="px-2">
            <v-text-field
              v-model="search"
              density="comfortable"
              placeholder="Search"
              prepend-inner-icon="mdi-magnify"
              style="max-width: 300px"
              variant="solo"
              clearable
              hide-details
            ></v-text-field>
          </v-toolbar>
        </template>
  
        <template v-slot:default="{ items }">
          <v-container class="pa-2" fluid>
            <v-row dense>
              <v-col v-for="item in items" :key="item.id" cols="auto" md="4">
  
  
  
                <v-card class="mx-auto" max-width="344" elevation="2">
                  <v-img height="200px" :src="item.raw.imagesURLs" cover> </v-img>
  
                  <v-card-title> {{item.raw.name}} </v-card-title>
  
                  <v-card-subtitle> ${{item.raw.price}} </v-card-subtitle>
  
                  <v-card-actions>
                    <v-btn
                      prepend-icon="mdi-plus"
                      color="primary"
                      text="Añadir al carrito"
                    ></v-btn>
  
                    <v-spacer></v-spacer>
  
                    <v-btn
                      :icon="show ? 'mdi-chevron-up' : 'mdi-chevron-down'"
                      @click="show = !show"
                    ></v-btn>
                  </v-card-actions>
  
                  <v-expand-transition>
                    <div v-show="show">
                      <v-divider></v-divider>
                      <v-spacer></v-spacer>
                      <v-chip-group>
                        <v-chip> Naturaleza </v-chip><v-chip> Comida </v-chip>
                      </v-chip-group>
                      <v-card-text>
                        Cactus de calidad del sur. Producido naturamente.
                      </v-card-text>
                    </div>
                  </v-expand-transition>
                </v-card>
  
  
                
              </v-col>
            </v-row>
          </v-container>
        </template>
  
        <template v-slot:footer="{ page, pageCount, prevPage, nextPage }">
          <div class="d-flex align-center justify-center pa-4">
            <v-btn
              :disabled="page === 1"
              density="comfortable"
              icon="mdi-arrow-left"
              variant="tonal"
              rounded
              @click="prevPage"
            ></v-btn>
  
            <div class="mx-2 text-caption">
              Page {{ page }} of {{ pageCount }}
            </div>
  
            <v-btn
              :disabled="page >= pageCount"
              density="comfortable"
              icon="mdi-arrow-right"
              variant="tonal"
              rounded
              @click="nextPage"
            ></v-btn>
          </div>
        </template>
      </v-data-iterator>
    </v-card>
  </template>
  
  <script>
    export default {
      data: () => ({
        show: false,
        search: '',
        products: [
          {
            id: 'test1',
            name: 'Name1',
            price: 1000,
            description: 'Descripción de prueba',
            imagesURLs:
              'https://cdn.vuetifyjs.com/docs/images/graphics/games/5.png',
            tags: ['Etiqueta1', 'Etiqueta2'],
          },
          {
            id: 'test2',
            name: 'Name2',
            price: 1000,
            description: 'Descripción de prueba',
            imagesURLs:
              'https://cdn.vuetifyjs.com/docs/images/graphics/games/3.png',
            tags: ['Etiqueta1', 'Etiqueta2'],
          },
          {
            id: 'test3',
            name: 'Name3',
            price: 1000,
            description: 'Descripción de prueba',
            imagesURLs:
              'https://cdn.vuetifyjs.com/docs/images/graphics/games/2.png',
            tags: ['Etiqueta1', 'Etiqueta2'],
          },
          {
            id: 'test4',
            name: 'Name4',
            price: 1000,
            description: 'Descripción de prueba',
            imagesURLs:
              'https://cdn.vuetifyjs.com/docs/images/graphics/games/4.png',
            tags: ['Etiqueta1', 'Etiqueta2'],
          },
        ],
      }),
      methods: {
        toggleShow() {
          this.show = !this.show
        },
      },
    }
  </script>
  