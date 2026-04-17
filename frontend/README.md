# Svelte Starter Kit

This project was initialized with the Svelte Starter Kit

## Local setup

Run the script in the `scripts directory` for setting up the project:

```shell
./scripts/initial-setup.sh
```

## Features

### Message Bus

The `MessageBus` class is a framework-agnostic way of sending and receiving messages across discrete parts of the frontend. Any component can "subscribe" to a message, and fire a callback function whenever a message of a particular type is sent with the message payload as a parameter. This has many uses in modern web-app development, from storing user session data to firing off notification events to display a toast, or controlling unrelated pieces of the app remotely. See how it is used in various components in the repo for some examples of how this can be used.

#### Setup

By default, the `MessageBus` uses `localStorage` as a data persistence layer. If this is undesired, remove the `MessageBus.initialize` method call in the base `+layout.svelte` to remove it. This will cause the Message Bus to "forget" events each time the user reloads or leaves the page.

Alternatively, if a different way of persisting message bus data is preferred (for example, session storage instead of local storage), create a new implementation of `IStorageProvider` and replace the `getLocalStorageProvider` call in the root layout file with your new implementation.

### UI Components

The starter kit houses several UI components in the `src/lib/ui` directory. These components should all be accessible and function for what they are intended to do. The list of components available in the starter kit will continue to grow, and any user of the starter kit is encouraged to contribute back any new generic UI components they build back into the starter kit for use in future projects!

Note that all the components have intentionally "rough" designs. The starter kit is not interested in giving a full-fledged design system out of the box. Instead, each project should update the styles of these components and make them fit the needs of the project.

### API

The `baseAPI` class can serve as a basis for all of your project specific data fetching needs. The class should standardize how requests are made to an application backend, particularly for common tasks such as building authorization headers and converting responses into JS objects that can be used by the rest of the app.

The API class transforms requests based on a middleware system. Each middleware function is called in order, and can modify the request before it is sent, or the response before it is returned to the caller. This is useful for adding authorization headers and creating the full URL.

#### Setup

1.  If your project has a version of the API that you can safely develop against locally, update your default configuration. <br /> In `static/config/config.example.json`, replace the `baseUrl` value with the URL of the local version of your API.
2.  If your API needs some kind of authorization token, set up the logic for including that in requests in the `AuthorizationMiddleware` class. If it is a bearer token, then all you need to do is update the `getBearerToken` method with however you are storing the bearer token (ie: if a logged-in user has a bearer token, you might store that in the message bus). Additionally, there are unit tests in `baseApi.spec.ts` covering ensuring that the auth header is correctly built. Update these tests to properly test that your auth headers are being built once you have an implementation in place.
3.  If any additional middleware functionality is needed, follow these steps: 4. Create a new class that implements the IRequestMiddleware interface. 5. Add the new middleware class to the registered middlewares by calling the `addMiddleware` method in the constructor. 6. If this middleware should run on all API requests, add it to the `baseApi` class's constructor. 7. If this middleware should only run on a specific (set of) API requests, add it to a derived class of `baseApi` that is specific to that set of requests.

### Runtime Configuration

The starter kit has a default way of retrieving run-time configuration on page load and storing this in the message bus. Any usage of configuration in the app should go through the `ConfigService` class for consistency and type-safety.

If you are using the starter kit's Dockerfile to deploy the app, you can bind mount the config file to the nginx html folder, for example: `/usr/share/nginx/html/config/config.json`

#### Setup

Update the `ApplicationConfig` type to match the configuration structure of your project. Make sure to keep the example config file (`static/config/config.example.json`) up to date with a good starting point for local development!

It is usually a good idea to re-run `scripts/initial-setup.sh` after making any changes to the example config JSON file, so that those changes are reflected in your local development environment.

At runtime, the application will be looking for `config/config.json` for its runtime config.

### Feature Flags

The starter kit has a basic feature flag system in place. This system allows for the enabling and disabling of features in the app based on a configuration file. This is useful for enabling and disabling features in the app without needing to deploy new code. The feature flag system takes in a provider that can be swapped out for a different implementation if desired. The default implementation makes use of the runtime configuration and `config.json` file to store the feature flags for the given environment. There is also a mock provider that can be used for testing purposes.

#### Setup

1. If desired, create a new implementation of the `IFeatureFlagProvider` interface, and replace the `FeatureFlagService`'s provider with your new implementation within the root layout file.
2. However you are storing your feature flags, ensure that each feature flag has at minimum a `name` and a `isEnabled` property. You may find it useful to keep a static dictionary of all possible feature flags. One such dictionary is started in `FeatureFlags.ts` in the `src/lib/services/FeatureFlag` directory.
3. If using the default implementation, it is likely a good idea to keep the `config.example.json` file up to date with a listing of all possible feature flags and their default values.

#### Testing with Feature Flags

Before your test run, initialize the feature flag service with the mock provider. Code example below:

```ts
beforeEach(async () => {
	let provider = new TestFeatureFlagProvider([
		createTestFeatureFlag('test1'),
		createTestFeatureFlag('test2', false)
	]);

	await FeatureFlagService.initialize(provider);
});
```

#### Using Feature Flags in Svelte Components

Because the feature flag service loads the data asynchronously on page load, it is likely that the feature flags will not always be loaded by the time the component is rendered. To handle this, the `FeatureFlagService` class's primary way of accessing feature flags is through a subscriber method. This subscriber should be called on an `onMount` hook within the Svelte component, like so:

```sveltehtml
<script lang="ts">
	import { onMount } from 'svelte';
	import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';
	import { FeatureFlags } from '$lib/services/FeatureFlag/FeatureFlags';

	let showDemo = false;

	onMount(() => {
		let unsubscribe = FeatureFlagService.subscribeToFeature(
			FeatureFlags.DEMO_FEATURE_FLAG,
			(value) => {
				showDemo = value;
			}
		);

		return () => {
			unsubscribe();
		};
	});
</script>
```

There is a utility component that abstracts most of this way called `FeatureToggle.svelte`. The component takes advantage of named slots to 'choose' which of its children to render. To render a child when the feature is enabled, provide the topmost child element a `slot="enabled"` attribute. To render a child when a feature is disabled, provide the topmost child element a `slot="disabled"` attribute. An example of how to use this component is below:

```sveltehtml
    <FeatureToggle featureFlag="test">
	  <p slot="enabled">Enabled</p>
	  <p slot="disabled">Disabled</p>
    </FeatureToggle>
```

### Deploying

The starter kit is set to build as a static site by default. This should work for websites and web apps with static URLs. If you need to use dynamic path parameters, or if you would prefer to build a server rendered app instead, follow the steps in the `Switch to a Server Rendered Deployment` section below.

#### Setup

Run `scripts/build-docker-image.sh` in your CI environment to build and push a new docker image (note that this script assumes certain environment files are present, look at the contents of the script for exact details). The image only exposes port 80, so it is recommended to put the deployed container behind a reverse proxy, such as traefik.

If you need to deploy in a non-docker environment, your CI environment should be able to run `npm run build:ci`, and copy the contents in the `./build` directory to a hosting environment.

#### Switch to a Server Rendered Deployment

1. In `package.json`, replace the `@sveltejs/adapter-static` dependency with `@sveltejs/adapter-node` (at the time of writing this, version `1.3.1` confirm works with the rest of the versions installed in this starter kit)
2. In `svelte.config.js`, replace the adapter import from `import adapter from '@sveltejs/adapter-static';` to `import adapter from '@sveltejs/adapter-node';`
3. Delete the layout file in `src/routes/+layout.ts`
4. Replace the `final` stage in the dockerfile (found at `docker/Dockerfile`) with the following contents:

```dockerfile
FROM node:18-alpine as final
COPY --from=builder app/build build/
COPY --from=builder app/node_modules node_modules/
COPY package.json .
ENV NODE_ENV=production
EXPOSE 3000
CMD ["node", "build"]
```

A few things to note:

- This exposes a different port (3000) than the static site build, so if you have any existing deployments, you may need to update the networking to accommodate this.
- If you have any bind mounts (for runtime config, for example), you will need to update these. For bind mounting the `config.json` file included in this starter kit, you would need to mount that to `/build/client/config/config.json`

### Sitemap

The starter kit will automatically generate a sitemap for your site. This is based on the file-based routing that SvelteKit is built around, and will include all pages that are generated by the SvelteKit router. The sitemap is, by default generated at build time, and is available at the `/sitemap.xml` path on your deployed site.

#### Setup

The sitemap is generated within `src/routes/sitemap.xml/+server.ts`. This file is a "server route" in SvelteKit, and is responsible for generating the sitemap.

By default, the sitemap will include every page within the app. If this is not desired, implement a way to filter out pages that should not be included within this file. Additionally, if you need to update the `changefreq` or `priority` of a specific page, you can do so by updating the `overrides` object in the same file.

Note that this implementation is very basic, and may not be suitable for all use cases. If you need a more advanced sitemap, or need to generate a sitemap based on dynamic data, you may need to implement additional behavior on top of this.
