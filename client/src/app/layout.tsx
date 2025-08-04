"use client";

import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { SideBar, NavBar } from "../components/ui/Layout";
import Providers from "../lib/react_query/providers";
import { NotificationProvider } from "@/components/ui/Feedback/NotificationProvider";
import { AuthProvider } from "@/features/auth/context/AuthContext";
import { usePathname } from "next/navigation";

const geistSans = Geist({ variable: "--font-geist-sans", subsets: ["latin"] });

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export default function RootLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  const pathname = usePathname();
  const isAuthPage = pathname === "/login" || pathname === "/register";

  return (
    <html lang="en">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased bg-white overflow-hidden`}
      >
        <Providers>
          <AuthProvider>
            <NotificationProvider>
              {isAuthPage ? (
                <div className="min-h-screen flex items-center justify-center bg-gray-50">
                  {children}
                </div>
              ) : (
                <>
                  <NavBar />
                  <SideBar />
                  <main className="fixed top-16 left-64 right-0 bottom-0 overflow-auto">
                    {children}
                  </main>
                </>
              )}
            </NotificationProvider>
          </AuthProvider>
        </Providers>
      </body>
    </html>
  );
}
