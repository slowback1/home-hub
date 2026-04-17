<div class="subnav">
	<a href="/demo/form">Form</a>
	<a href="/demo/list">List</a>
	<a href="/demo/content">Content</a>
</div>

<slot />

<style>
	.subnav {
		display: flex;
		justify-content: center;
		margin-bottom: 1rem;
	}

	.subnav a {
		margin: 0 1rem;
	}
</style>
