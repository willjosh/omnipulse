import axios from "axios";

const sleep = (delay: number) => {
  return new Promise(resolve => {
    setTimeout(resolve, delay);
  });
};

const baseURL =
  process.env.NODE_ENV === "production"
    ? "https://omnipulse-backend.wonderfulsky-7bfd34c0.australiaeast.azurecontainerapps.io"
    : "http://localhost:5100";

export const agent = axios.create({ baseURL });

agent.interceptors.response.use(async response => {
  try {
    await sleep(1000);
    return response;
  } catch (error) {
    console.log(error);
    return Promise.reject(error);
  }
});
