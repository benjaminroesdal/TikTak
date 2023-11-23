import { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'io.ionic.starter',
  appName: 'TikTakClient',
  webDir: 'www',
  server: {
    androidScheme: 'https'
  },
  plugins: {
    GoogleAuth: {
      scopes: ['profile', 'email'],
      serverClientId: '69308154140-5lk0usbipr9j6es0v9pg4tvkj84nn7sf.apps.googleusercontent.com',
      forceCodeForRefreshToken: true,
    }
  }
};

export default config;
