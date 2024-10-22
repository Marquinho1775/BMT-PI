<template>
	<v-card class="mx-auto" max-width="344" elevation="4">
		<!-- Carrousel de Imágenes -->
		<v-carousel show-arrows="hover" height="200px" hide-delimiters>
			<v-carousel-item v-for="(image, index) in product.imagesURLs" :key="index">
				<v-img :src="image" height="200px" aspect-ratio="16/9" cover></v-img>
			</v-carousel-item>
		</v-carousel>

		<!-- Título y Precio del Producto -->
		<v-card-title>{{ product.name }}</v-card-title>
		<v-card-subtitle>₡ {{ product.price }}</v-card-subtitle>

		<v-card-actions>
			<v-btn prepend-icon="mdi-plus" color="primary" text>Añadir al carrito</v-btn>
			<v-spacer></v-spacer>
			<v-btn :icon="isShow ? 'mdi-chevron-up' : 'mdi-chevron-down'" @click="toggleShow"></v-btn>
		</v-card-actions>

		<!-- Tags y descripción -->
		<v-expand-transition>
			<div v-show="isShow">
				<v-divider></v-divider>
				<v-spacer></v-spacer>
				<v-chip-group>
					<v-chip v-for="(tag, index) in product.tags" :key="index" class="mr-4">{{ tag }}</v-chip>
				</v-chip-group>
				<v-card-text>{{ product.description }}</v-card-text>
			</div>
		</v-expand-transition>
	</v-card>

</template>

<script>
export default {
	name: 'ProductCard',
	props: {
		product: {
			type: Object,
			required: true,
		},
	},
	data() {
		return {
			isShow: false, // Estado local para manejar la visibilidad
		};
	},
	mounted() {
		console.log('ProductCard montado con producto:', this.product);
	},
	methods: {
		toggleShow() {
			this.isShow = !this.isShow;
		},
	},
};
</script>

<style scoped>
.v-card {
	margin-bottom: 16px;
}
</style>