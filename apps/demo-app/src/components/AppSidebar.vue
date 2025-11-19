<script setup lang="ts">
import type { SidebarProps } from '@/components/ui/sidebar'

import { LayoutDashboard, Users, MapPin, Settings } from 'lucide-vue-next'
import NavMain from '@/components/NavMain.vue'

import {
  Sidebar,
  SidebarContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarRail,
} from '@/components/ui/sidebar'

const props = withDefaults(defineProps<SidebarProps>(), {
  collapsible: 'icon',
})

const data = {
  navMain: [
    {
      title: 'Dashboard',
      url: '#dashboard',
      icon: LayoutDashboard,
      isActive: true,
    },
    {
      title: 'Employees',
      url: '#employees',
      icon: Users,
      isActive: false,
    },
    {
      title: 'Locations',
      url: '#locations',
      icon: MapPin,
      isActive: false,
    },
    {
      title: 'Settings',
      url: '#settings',
      icon: Settings,
      isActive: false,
    },
  ],
}
</script>

<template>
  <Sidebar v-bind="props">
    <SidebarHeader>
      <SidebarMenu>
        <SidebarMenuItem>
          <SidebarMenuButton size="lg" as-child>
            <a href="#">
              <div class="flex aspect-square size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground">
                <LayoutDashboard class="size-4" />
              </div>
              <div class="grid flex-1 text-left text-sm leading-tight">
                <span class="truncate font-semibold">Demo App</span>
                <span class="truncate text-xs">Organization Portal</span>
              </div>
            </a>
          </SidebarMenuButton>
        </SidebarMenuItem>
      </SidebarMenu>
    </SidebarHeader>
    <SidebarContent>
      <NavMain :items="data.navMain" />
    </SidebarContent>
    <SidebarRail />
  </Sidebar>
</template>
